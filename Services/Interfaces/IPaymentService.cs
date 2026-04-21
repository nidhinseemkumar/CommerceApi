using CommerceApi.Models;

namespace CommerceApi.Services;

public interface IPaymentService
{
    Task<Payment> ProcessPaymentAsync(int userId, CommerceApi.DTOs.PaymentDto dto);
    Payment GetPaymentByOrder(int userId, int orderId);
}
