using System.Text.Json.Serialization;

namespace TelegramPanel.Modules;

public sealed class ModuleManifest
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "0.0.0";

    [JsonPropertyName("host")]
    public HostCompatibility Host { get; set; } = new();

    [JsonPropertyName("dependencies")]
    public List<ModuleDependency> Dependencies { get; set; } = new();

    [JsonPropertyName("entry")]
    public ModuleEntryPoint Entry { get; set; } = new();
}

public sealed class HostCompatibility
{
    [JsonPropertyName("min")]
    public string? Min { get; set; }

    [JsonPropertyName("max")]
    public string? Max { get; set; }
}

public sealed class ModuleDependency
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    /// <summary>
    /// 版本范围表达式，支持：
    /// - 1.2.3（等于）
    /// - &gt;=1.2.3
    /// - &gt;=1.2.3 &lt;2.0.0（空格分隔多个条件）
    /// </summary>
    [JsonPropertyName("range")]
    public string Range { get; set; } = "";
}

public sealed class ModuleEntryPoint
{
    [JsonPropertyName("assembly")]
    public string Assembly { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
}

