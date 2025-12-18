using TelegramPanel.Data.Entities;
using TelegramPanel.Data.Repositories;

namespace TelegramPanel.Core.Services;

/// <summary>
/// 账号分类管理服务
/// </summary>
public class AccountCategoryManagementService
{
    private readonly IAccountCategoryRepository _categoryRepository;

    public AccountCategoryManagementService(IAccountCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<AccountCategory>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<AccountCategory?> GetCategoryAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<AccountCategory?> GetCategoryByNameAsync(string name)
    {
        return await _categoryRepository.GetByNameAsync(name);
    }

    public async Task<AccountCategory> CreateCategoryAsync(AccountCategory category)
    {
        return await _categoryRepository.AddAsync(category);
    }

    public async Task UpdateCategoryAsync(AccountCategory category)
    {
        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category != null)
        {
            await _categoryRepository.DeleteAsync(category);
        }
    }
}
