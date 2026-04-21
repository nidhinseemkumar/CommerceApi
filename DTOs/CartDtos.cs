using CommerceApi.Models;

namespace CommerceApi.DTOs;

public record AddToCartDto(int ProductId, int Quantity);

public class CartSummaryDto
{
    public List<CartItemDetailDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public List<Product> RecommendedProducts { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class CartItemDetailDto
{
    public int CartItemId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
}
