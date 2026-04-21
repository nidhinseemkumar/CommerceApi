using CommerceApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CommerceApi.Services;

public class CachedCategoryService(CategoryService inner, IMemoryCache cache) : ICategoryService
{
    private const string AllCategoriesKey = "Categories_All";
    private static string CategoryKey(int id) => $"Category_{id}";

    private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromMinutes(10))
        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

    public async Task<List<Category>> GetAllAsync()
    {
        if (cache.TryGetValue(AllCategoriesKey, out List<Category>? categories))
        {
            return categories!;
        }

        categories = await inner.GetAllAsync();
        cache.Set(AllCategoriesKey, categories, _cacheOptions);
        return categories;
    }

    public Category GetById(int id)
    {
        string key = CategoryKey(id);
        if (cache.TryGetValue(key, out Category? category))
        {
            return category!;
        }

        category = inner.GetById(id);
        cache.Set(key, category, _cacheOptions);
        return category;
    }

    public async Task<Category> CreateAsync(Category category)
    {
        var result = await inner.CreateAsync(category);
        InvalidateCache(result.Id);
        return result;
    }

    public async Task<Category> UpdateAsync(int id, Category updated)
    {
        var result = await inner.UpdateAsync(id, updated);
        InvalidateCache(id);
        return result;
    }

    public async Task DeleteAsync(int id)
    {
        await inner.DeleteAsync(id);
        InvalidateCache(id);
    }

    private void InvalidateCache(int id)
    {
        cache.Remove(AllCategoriesKey);
        cache.Remove(CategoryKey(id));
        // Categories often affect product lists, so we clear product cache too for safety if needed
        cache.Remove("Products_All");
    }
}
