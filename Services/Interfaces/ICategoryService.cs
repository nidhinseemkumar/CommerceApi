using CommerceApi.Models;

namespace CommerceApi.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Category GetById(int id);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(int id, Category updated);
    Task DeleteAsync(int id);
}
