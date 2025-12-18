using TelegramPanel.Data.Entities;

namespace TelegramPanel.Data.Repositories;

/// <summary>
/// 账号分类仓储接口
/// </summary>
public interface IAccountCategoryRepository : IRepository<AccountCategory>
{
    Task<AccountCategory?> GetByNameAsync(string name);
}
