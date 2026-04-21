using CommerceApi.Models;

namespace CommerceApi.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync();
    List<Order> GetUserOrders(int userId);
    Task<Order> PlaceOrderAsync(int userId, CommerceApi.DTOs.PlaceOrderDto dto);
}
