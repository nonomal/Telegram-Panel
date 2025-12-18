using TelegramPanel.Data.Entities;

namespace TelegramPanel.Data.Repositories;

/// <summary>
/// 频道分组仓储接口
/// </summary>
public interface IChannelGroupRepository : IRepository<ChannelGroup>
{
    Task<ChannelGroup?> GetByNameAsync(string name);
}
