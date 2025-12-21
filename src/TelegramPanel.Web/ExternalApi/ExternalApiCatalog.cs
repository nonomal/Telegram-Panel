namespace TelegramPanel.Web.ExternalApi;

public static class ExternalApiCatalog
{
    public static readonly ExternalApiTypeInfo Kick = new(
        Type: ExternalApiTypes.Kick,
        DisplayName: "踢人/封禁",
        Route: "/api/kick",
        ProviderModuleId: "builtin.kick-api");

    public static IReadOnlyList<ExternalApiTypeInfo> All { get; } = new[]
    {
        Kick
    };

    public static ExternalApiTypeInfo? TryGet(string? type)
    {
        type = (type ?? string.Empty).Trim();
        if (type.Length == 0)
            return null;

        return All.FirstOrDefault(x => string.Equals(x.Type, type, StringComparison.OrdinalIgnoreCase));
    }
}

public sealed record ExternalApiTypeInfo(
    string Type,
    string DisplayName,
    string Route,
    string ProviderModuleId);

