namespace CommerceApi.Models;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;  // Card, UPI, COD
    public string Status { get; set; } = "Pending";      // Pending, Completed, Failed
    public DateTime? PaidAt { get; set; }
}