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
public class OrdersController(IOrderService orderService, IFileService fileService) : ControllerBase
{
    [HttpGet("export/{format}")]
    public IActionResult Export(string format)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var orders = orderService.GetUserOrders(userId);
        return ProcessExport(orders, format, "my_orders");
    }

    [HttpGet("/api/admin/orders/export/{format}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportAll(string format)
    {
        var orders = await orderService.GetAllAsync();
        return ProcessExport(orders, format, "all_orders");
    }

    private IActionResult ProcessExport(List<Order> orders, string format, string filename)
    {
        var data = orders.Select(o => new OrderExportDto
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            UserEmail = o.User?.Email ?? "Unknown",
            TotalAmount = o.TotalAmount,
            ItemCount = o.Items.Count
        });

        if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = fileService.ExportToCsv(data);
            return File(bytes, "text/csv", $"{filename}.csv");
        }
        else if (format.Equals("excel", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = fileService.ExportToExcel(data, "Orders");
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{filename}.xlsx");
        }

        return BadRequest("Invalid format. Use 'csv' or 'excel'.");
    }
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