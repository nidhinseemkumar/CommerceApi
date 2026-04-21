using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CommerceApi.Services;
using CommerceApi.DTOs;
using CommerceApi.Models;
using System.Security.Claims;

namespace CommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartsController(IServiceScopeFactory scopeFactory) : ControllerBase
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetCartSummary()
    {
        var summary = new CartSummaryDto();
        var tasks = new List<Task>();

        // 1. Fetch Cart Items
        var cartTask = Task.Run(async () => 
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICartService>();
            var items = await service.GetCartItemsAsync(UserId);
            
            summary.Items = items.Select(i => new CartItemDetailDto
            {
                CartItemId = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "Unknown",
                UnitPrice = i.Product?.Price ?? 0,
                Quantity = i.Quantity
            }).ToList();
            
            summary.TotalAmount = summary.Items.Sum(i => i.Subtotal);
        });

        // 2. Fetch Personalized Recommendations (Parallel Activity)
        var recommendationsTask = Task.Run(async () => 
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IProductService>();
            var allProducts = await service.GetAllAsync();
            // Just a simple recommendation logic for demo
            summary.RecommendedProducts = allProducts.Take(3).ToList();
        });

        tasks.Add(cartTask);
        tasks.Add(recommendationsTask);

        await Task.WhenAll(tasks);

        summary.Message = "Cart loaded with recommendations in parallel.";
        return Ok(summary);
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ICartService>();
        await service.AddToCartAsync(UserId, dto.ProductId, dto.Quantity);
        return Ok("Item added to cart.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ICartService>();
        await service.RemoveFromCartAsync(UserId, id);
        return NoContent();
    }

    [HttpPost("clear")]
    public async Task<IActionResult> ClearCart()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ICartService>();
        await service.ClearCartAsync(UserId);
        return Ok("Cart cleared.");
    }
}
