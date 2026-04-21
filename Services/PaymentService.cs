using CommerceApi.Data;
using CommerceApi.Exceptions;
using CommerceApi.Models;
using CommerceApi.DTOs;

namespace CommerceApi.Services;

public interface IPaymentService
{
    Task<Payment> ProcessPaymentAsync(int userId, CommerceApi.DTOs.PaymentDto dto);
    Payment GetPaymentByOrder(int userId, int orderId);
}

public class PaymentService(AppDbContext db) : IPaymentService
{
    public async Task<Payment> ProcessPaymentAsync(int userId, CommerceApi.DTOs.PaymentDto dto)
    {
        var order = db.Orders.FirstOrDefault(o => o.Id == dto.OrderId && o.UserId == userId);
        if (order == null)
            throw new NotFoundException("Order not found or doesn't belong to you.");

        var existing = db.Payments.FirstOrDefault(p => p.OrderId == dto.OrderId && p.Status == "Completed");
        if (existing != null)
            throw new BadRequestException("This order has already been paid.");

        var payment = new Payment
        {
            OrderId = dto.OrderId,
            Amount = order.TotalAmount,
            Method = dto.Method,
            Status = "Completed",
            PaidAt = DateTime.Now
        };

        db.Payments.Add(payment);
        await db.SaveChangesAsync();
        return payment;
    }

    public Payment GetPaymentByOrder(int userId, int orderId)
    {
        var order = db.Orders.FirstOrDefault(o => o.Id == orderId && o.UserId == userId);
        if (order == null) throw new NotFoundException("Order not found.");

        var payment = db.Payments.FirstOrDefault(p => p.OrderId == orderId);
        if (payment == null) throw new NotFoundException("No payment found for this order.");

        return payment;
    }
}
