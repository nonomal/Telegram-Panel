using Microsoft.EntityFrameworkCore;
using TelegramPanel.Data.Entities;

namespace TelegramPanel.Data.Repositories;

/// <summary>
/// 账号分类仓储实现
/// </summary>
public class AccountCategoryRepository : Repository<AccountCategory>, IAccountCategoryRepository
{
    public AccountCategoryRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<AccountCategory>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.Accounts)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<AccountCategory?> GetByNameAsync(string name)
    {
        return await _dbSet
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Name == name);
    }
}
