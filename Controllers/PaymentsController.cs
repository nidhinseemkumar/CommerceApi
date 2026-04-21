using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CommerceApi.Data;
using CommerceApi.Models;
using CommerceApi.Services;
using CommerceApi.DTOs;

namespace CommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    // Make a payment for an order
    [HttpPost]
    public async Task<IActionResult> Pay([FromBody] CommerceApi.DTOs.PaymentDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await paymentService.ProcessPaymentAsync(userId, dto);
            return Ok(result);
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Payment processing failed: " + ex.Message);
        }
    }

    // Get payment status for an order
    [HttpGet("order/{orderId}")]
    public IActionResult GetByOrder(int orderId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(paymentService.GetPaymentByOrder(userId, orderId));
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }
}