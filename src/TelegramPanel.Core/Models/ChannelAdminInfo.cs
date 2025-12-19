namespace TelegramPanel.Core.Models;

public record ChannelAdminInfo(
    long UserId,
    string? Username,
    string? FirstName,
    string? LastName,
    bool IsCreator,
    string? Rank
)
{
    public string DisplayName
    {
        get
        {
            var full = $"{FirstName} {LastName}".Trim();
            if (!string.IsNullOrWhiteSpace(full))
                return full;
            if (!string.IsNullOrWhiteSpace(Username))
                return $"@{Username}";
            return UserId.ToString();
        }
    }
}

