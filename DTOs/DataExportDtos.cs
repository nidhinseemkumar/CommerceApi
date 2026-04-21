namespace CommerceApi.DTOs;

public class ProductExportDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? CategoryName { get; set; }
}

public class OrderExportDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int ItemCount { get; set; }
}
