namespace TelegramPanel.Core.Services.Telegram;

internal static class TelegramRpcErrorTranslator
{
    public static string? TranslateOwnershipTransferWaitError(string text)
    {
        if (TryGetRpcWaitSeconds(text, "FLOOD_WAIT_", out var floodWaitSeconds))
            return $"Telegram 风控限流：操作过于频繁，需要等待 {FormatWaitDuration(floodWaitSeconds)} 后再重试。（{text}）";
        if (text.Contains("FLOOD_WAIT", StringComparison.OrdinalIgnoreCase))
            return $"Telegram 风控限流：操作过于频繁，请降低频率并稍后重试。（{text}）";

        if (TryGetRpcWaitSeconds(text, "PASSWORD_TOO_FRESH_", out var passwordWaitSeconds))
            return $"Telegram 安全限制：当前账号二级密码近期有变动，需要等待 {FormatWaitDuration(passwordWaitSeconds)} 后才能转让所有权。（{text}）";
        if (text.Contains("PASSWORD_TOO_FRESH", StringComparison.OrdinalIgnoreCase))
            return $"Telegram 安全限制：当前账号二级密码近期有变动，需要等待一段时间后才能转让所有权。（{text}）";

        if (TryGetRpcWaitSeconds(text, "SESSION_TOO_FRESH_", out var sessionWaitSeconds))
            return $"Telegram 安全限制：当前账号登录会话太新，需要等待 {FormatWaitDuration(sessionWaitSeconds)} 后才能转让所有权。（{text}）";
        if (text.Contains("SESSION_TOO_FRESH", StringComparison.OrdinalIgnoreCase))
            return $"Telegram 安全限制：当前账号登录会话太新，需要等待一段时间后才能转让所有权。（{text}）";

        return null;
    }

    public static bool TryGetRpcWaitSeconds(string message, string prefix, out int seconds)
    {
        seconds = 0;
        if (string.IsNullOrWhiteSpace(message))
            return false;

        var start = message.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
            return false;

        start += prefix.Length;
        var end = start;
        while (end < message.Length && char.IsDigit(message[end]))
            end++;

        if (end == start)
            return false;

        if (!int.TryParse(message.AsSpan(start, end - start), out seconds))
            return false;

        return seconds > 0;
    }

    private static string FormatWaitDuration(int seconds)
    {
        if (seconds < 60)
            return $"{seconds} 秒";

        var span = TimeSpan.FromSeconds(seconds);
        if (span.TotalHours < 1)
            return $"{Math.Ceiling(span.TotalMinutes):0} 分钟";
        if (span.TotalDays < 1)
            return $"{Math.Ceiling(span.TotalHours):0} 小时";

        return $"{Math.Ceiling(span.TotalDays):0} 天";
    }
}
