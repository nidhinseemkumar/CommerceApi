using CommerceApi.Models;

namespace CommerceApi.Services;

public interface ICartService
{
    Task<List<CartItem>> GetCartItemsAsync(int userId);
    Task AddToCartAsync(int userId, int productId, int quantity);
    Task RemoveFromCartAsync(int userId, int cartItemId);
    Task ClearCartAsync(int userId);
}
