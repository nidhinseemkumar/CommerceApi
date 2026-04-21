namespace CommerceApi.DTOs;

public record PlaceOrderDto(List<OrderItemDto> Items);
public record OrderItemDto(int ProductId, int Quantity);
