using CommerceApi.Models;

namespace CommerceApi.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Product GetById(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(int id, Product updated);
    Task DeleteAsync(int id);
}
