using CommerceApi.Models;
using CommerceApi.Data;
using CommerceApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CommerceApi.Services;

public class CategoryService(AppDbContext db) : ICategoryService
{
    public List<Category> GetAll() =>
        db.Categories.Include(c => c.Products).ToList();

    public async Task<List<Category>> GetAllAsync() 
    {
        Console.WriteLine("[DB] Fetching all categories from database...");
        return await db.Categories.Include(c => c.Products).ToListAsync();
    }

    public Category GetById(int id)
    {
        Console.WriteLine($"[DB] Fetching category {id} from database...");
        var c = db.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == id);
        if (c == null) throw new NotFoundException("Category not found.");
        return c;
    }

    public async Task<Category> CreateAsync(Category category)
    {
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(int id, Category updated)
    {
        var c = db.Categories.Find(id);
        if (c == null) throw new NotFoundException("Category not found.");
        c.Name = updated.Name;
        c.Description = updated.Description;
        await db.SaveChangesAsync();
        return c;
    }

    public async Task DeleteAsync(int id)
    {
        var c = db.Categories.Find(id);
        if (c == null) throw new NotFoundException("Category not found.");
        db.Categories.Remove(c);
        await db.SaveChangesAsync();
    }
}
