namespace TelegramPanel.Web.ExternalApi;

using System.Text.Json.Nodes;

public static class ExternalApiTypes
{
    public const string Kick = "kick";
}

public sealed class ExternalApiDefinition
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public string Type { get; set; } = ExternalApiTypes.Kick;
    public bool Enabled { get; set; }
    public string ApiKey { get; set; } = "";

    /// <summary>
    /// 模块自定义配置（JSON object）。由具体 API 类型自行解释。
    /// </summary>
    public JsonObject Config { get; set; } = new();

    /// <summary>
    /// 兼容内置 kick 的强类型配置（建议同时写入 Config）。
    /// </summary>
    public KickApiDefinition? Kick { get; set; } = new();
}

public sealed class KickApiDefinition
{
    public int BotId { get; set; } // 0=all bots
    public bool UseAllChats { get; set; } = true;
    public List<long> ChatIds { get; set; } = new();
    public bool PermanentBanDefault { get; set; }
}

