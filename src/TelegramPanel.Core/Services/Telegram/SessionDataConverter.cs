using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WTelegram;

namespace TelegramPanel.Core.Services.Telegram;

internal static class SessionDataConverter
{
    public static async Task<bool> TryConvertSessionFromSessionDataAsync(
        string phone,
        int apiId,
        string apiHash,
        string targetSessionPath,
        ILogger logger)
    {
        try
        {
            var jsonPath = TryFindSessionDataJson(phone);
            if (string.IsNullOrWhiteSpace(jsonPath))
                return false;

            var jsonText = await File.ReadAllTextAsync(jsonPath);
            using var doc = JsonDocument.Parse(jsonText);

            if (!doc.RootElement.TryGetProperty("session_string", out var sessionProp) || sessionProp.ValueKind != JsonValueKind.String)
                return false;

            var sessionString = sessionProp.GetString();
            if (string.IsNullOrWhiteSpace(sessionString))
                return false;

            var sessionBytes = DecodeBase64Url(sessionString.Trim());
            if (sessionBytes.Length == 0)
                return false;

            // 备份旧文件（如果存在）
            TryBackupIfExists(targetSessionPath, suffix: "bak");

            var sessionsDir = Path.GetDirectoryName(Path.GetFullPath(targetSessionPath)) ?? Directory.GetCurrentDirectory();
            Directory.CreateDirectory(sessionsDir);
            await File.WriteAllBytesAsync(targetSessionPath, sessionBytes);

            // 用 WTelegram 连接一次验证 session 是否可用
            string Config(string what) => what switch
            {
                "api_id" => apiId.ToString(),
                "api_hash" => apiHash,
                "session_pathname" => targetSessionPath,
                _ => null!
            };

            using var client = new Client(Config);
            await client.ConnectAsync();
            if (client.User == null)
                return false;

            logger.LogInformation("Converted session for {Phone} from session数据 json: {JsonPath}", phone, jsonPath);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to convert session for {Phone} from session数据 json", phone);
            return false;
        }
    }

    public static bool LooksLikeSqliteSession(string filePath)
    {
        try
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Span<byte> header = stackalloc byte[16];
            var read = fs.Read(header);
            if (read < 15) return false;
            var text = Encoding.ASCII.GetString(header[..15]);
            return string.Equals(text, "SQLite format 3", StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }

    private static string? TryFindSessionDataJson(string phone)
    {
        var current = Directory.GetCurrentDirectory();
        for (int i = 0; i < 6 && !string.IsNullOrWhiteSpace(current); i++)
        {
            var candidate = Path.Combine(current, "session数据", phone, $"{phone}.json");
            if (File.Exists(candidate))
                return candidate;

            var dirCandidate = Path.Combine(current, "session数据", phone);
            if (Directory.Exists(dirCandidate))
            {
                var any = Directory.EnumerateFiles(dirCandidate, "*.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(any))
                    return any;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        return null;
    }

    private static byte[] DecodeBase64Url(string input)
    {
        var s = input.Replace('-', '+').Replace('_', '/');
        var mod = s.Length % 4;
        if (mod == 2) s += "==";
        else if (mod == 3) s += "=";

        try
        {
            return Convert.FromBase64String(s);
        }
        catch
        {
            return Array.Empty<byte>();
        }
    }

    private static void TryBackupIfExists(string path, string suffix)
    {
        try
        {
            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
                return;

            var dir = Path.GetDirectoryName(fullPath) ?? Directory.GetCurrentDirectory();
            var name = Path.GetFileNameWithoutExtension(fullPath);
            var ext = Path.GetExtension(fullPath);
            var backupPath = Path.Combine(dir, $"{name}.{suffix}{ext}");
            File.Move(fullPath, backupPath, overwrite: true);
        }
        catch
        {
            // 忽略备份失败
        }
    }
}

