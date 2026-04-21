namespace CommerceApi.DTOs;

public record LoginDto(string Username, string Password);
public record RegisterDto(string Username, string Password, string Email, string PhoneNumber, string Address, string Role);
