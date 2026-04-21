using CommerceApi.Models;

namespace CommerceApi.DTOs;

public class DashboardSummaryDto
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalOrders { get; set; }
    public List<Order> RecentOrders { get; set; } = new();
    public List<Product> LatestProducts { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
