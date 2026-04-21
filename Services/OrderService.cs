using CommerceApi.Data;
using CommerceApi.Exceptions;
using CommerceApi.Models;
using CommerceApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CommerceApi.Services;

public class OrderService(AppDbContext db) : IOrderService
{
    public async Task<List<Order>> GetAllAsync() => 
        await db.Orders.Include(o => o.User).Include(o => o.Items).ToListAsync();
    public List<Order> GetUserOrders(int userId)
    {
        return db.Orders
            .Include(o => o.User)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId).ToList();
    }

    public async Task<Order> PlaceOrderAsync(int userId, CommerceApi.DTOs.PlaceOrderDto dto)
    {
        if (dto?.Items == null || !dto.Items.Any())
            throw new BadRequestException("Order items cannot be empty.");

        decimal total = 0;
        var items = new List<OrderItem>();

        foreach (var item in dto.Items)
        {
            var product = await db.Products.FindAsync(item.ProductId);

            if (product == null)
                throw new NotFoundException($"Product with ID {item.ProductId} does not exist.");

            if (product.Stock < item.Quantity)
                throw new BadRequestException($"Not enough stock for '{product.Name}'. Available: {product.Stock}, Requested: {item.Quantity}.");

            product.Stock -= item.Quantity;
            total += product.Price * item.Quantity;
            items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        var order = new Order { UserId = userId, TotalAmount = total, Items = items };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return await db.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstAsync(o => o.Id == order.Id);
    }
}
