using CommerceApi.Models;
using CommerceApi.Data;
using CommerceApi.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CommerceApi.Services;

public class CartService(AppDbContext db) : ICartService
{
    public async Task<List<CartItem>> GetCartItemsAsync(int userId)
    {
        return await db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        var product = await db.Products.FindAsync(productId);
        if (product == null) throw new NotFoundException("Product not found.");

        var existingItem = await db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            db.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            });
        }

        await db.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(int userId, int cartItemId)
    {
        var item = await db.CartItems.FindAsync(cartItemId);
        if (item == null || item.UserId != userId)
            throw new NotFoundException("Cart item not found.");

        db.CartItems.Remove(item);
        await db.SaveChangesAsync();
    }

    public async Task ClearCartAsync(int userId)
    {
        var items = await db.CartItems.Where(c => c.UserId == userId).ToListAsync();
        db.CartItems.RemoveRange(items);
        await db.SaveChangesAsync();
    }
}
