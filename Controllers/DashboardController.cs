using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CommerceApi.Services;
using CommerceApi.DTOs;
using CommerceApi.Models;

namespace CommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController(IServiceScopeFactory scopeFactory) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = new DashboardSummaryDto();
        var tasks = new List<Task>();

        var productsTask = Task.Run(async () => 
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IProductService>();
            var products = await service.GetAllAsync();
            summary.TotalProducts = products.Count;
            summary.LatestProducts = products.OrderByDescending(p => p.Id).Take(5).ToList();
        });

        var categoriesTask = Task.Run(async () => 
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ICategoryService>();
            var categories = await service.GetAllAsync();
            summary.TotalCategories = categories.Count;
        });

        var ordersTask = Task.Run(async () => 
        {
            using var scope = scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOrderService>();
            var orders = await service.GetAllAsync();
            summary.TotalOrders = orders.Count;
            summary.RecentOrders = orders.OrderByDescending(o => o.Id).Take(5).ToList();
        });

        tasks.Add(productsTask);
        tasks.Add(categoriesTask);
        tasks.Add(ordersTask);

        try
        {
            await Task.WhenAll(tasks);
            summary.Message = "Dashboard data aggregated successfully in parallel.";
            return Ok(summary);
        }
        catch (Exception)
        {
            var errors = tasks.Where(t => t.IsFaulted)
                             .Select(t => t.Exception?.InnerException?.Message)
                             .ToList();

            return StatusCode(500, new 
            { 
                Message = "One or more tasks failed.", 
                Errors = errors,
                PartialData = summary 
            });
        }
    }

    [HttpGet("example-api-calls")]
    public async Task<IActionResult> MultiApiCallExample()
    {
        using var scope1 = scopeFactory.CreateScope();
        using var scope2 = scopeFactory.CreateScope();
        using var scope3 = scopeFactory.CreateScope();

        var t1 = scope1.ServiceProvider.GetRequiredService<IProductService>().GetAllAsync();
        var t2 = scope2.ServiceProvider.GetRequiredService<ICategoryService>().GetAllAsync();
        var t3 = scope3.ServiceProvider.GetRequiredService<IOrderService>().GetAllAsync();

        await Task.WhenAll(t1, t2, t3);

        return Ok(new
        {
            ProductCount = t1.Result.Count,
            CategoryCount = t2.Result.Count,
            OrderCount = t3.Result.Count
        });
    }
}
