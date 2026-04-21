using CommerceApi.Models;
using CommerceApi.Data;
using CommerceApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CommerceApi.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Product GetById(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(int id, Product updated);
    Task DeleteAsync(int id);
}

public class ProductService(AppDbContext db) : IProductService
{
    public List<Product> GetAll() 
    {
        Console.WriteLine("[DB] Fetching all products from database...");
        return db.Products.ToList();
    }

    public async Task<List<Product>> GetAllAsync() 
    {
        Console.WriteLine("[DB] Fetching all products from database (Async)...");
        return await db.Products.ToListAsync();
    }

    public Product GetById(int id)
    {
        Console.WriteLine($"[DB] Fetching product {id} from database...");
        var p = db.Products.Find(id);
        if (p == null) throw new NotFoundException("Product not found.");
        return p;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(int id, Product updated)
    {
        var p = GetById(id);
        p.Name = updated.Name;
        p.Price = updated.Price;
        p.Stock = updated.Stock;
        p.Description = updated.Description;
        await db.SaveChangesAsync();
        return p;
    }

    public async Task DeleteAsync(int id)
    {
        var p = GetById(id);
        db.Products.Remove(p);
        await db.SaveChangesAsync();
    }
}
