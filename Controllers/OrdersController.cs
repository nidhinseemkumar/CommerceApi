using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CommerceApi.Data;
using CommerceApi.Models;
using CommerceApi.Services;
using CommerceApi.DTOs;

namespace CommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetMyOrders()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return Ok(orderService.GetUserOrders(userId));
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] CommerceApi.DTOs.PlaceOrderDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await orderService.PlaceOrderAsync(userId, dto);
            return Ok(result);
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Placement failed: " + ex.Message);
        }
    }
}