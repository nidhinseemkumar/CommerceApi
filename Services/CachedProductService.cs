using CommerceApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CommerceApi.Services;

public class CachedProductService(ProductService inner, IMemoryCache cache) : IProductService
{
    private const string AllProductsKey = "Products_All";
    private static string ProductKey(int id) => $"Product_{id}";

    private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions()
        .SetSlidingExpiration(TimeSpan.FromMinutes(5))
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

    public async Task<List<Product>> GetAllAsync()
    {
        if (cache.TryGetValue(AllProductsKey, out List<Product>? products))
        {
            return products!;
        }

        products = await inner.GetAllAsync();
        cache.Set(AllProductsKey, products, _cacheOptions);
        return products;
    }

    public Product GetById(int id)
    {
        string key = ProductKey(id);
        if (cache.TryGetValue(key, out Product? product))
        {
            return product!;
        }

        product = inner.GetById(id);
        cache.Set(key, product, _cacheOptions);
        return product;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        var result = await inner.CreateAsync(product);
        InvalidateCache(result.Id);
        return result;
    }

    public async Task<Product> UpdateAsync(int id, Product updated)
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
        cache.Remove(AllProductsKey);
        cache.Remove(ProductKey(id));
    }
}
