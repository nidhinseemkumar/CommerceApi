using CommerceApi.Models;
using CommerceApi.DTOs;
using CommerceApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CommerceApi.Services;

public partial class AuthService
{
    public async Task RegisterAsync(CommerceApi.DTOs.RegisterDto dto)
    {
        if (db.Users.Any(u => u.Username == dto.Username))
            throw new Exceptions.ConflictException("Username already exists.");

        if (db.Users.Any(u => u.Email == dto.Email))
            throw new Exceptions.ConflictException("Email is already registered.");

        if (db.Users.Any(u => u.PhoneNumber == dto.PhoneNumber))
            throw new Exceptions.ConflictException("Phone number is already registered.");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            Role = string.IsNullOrEmpty(dto.Role) ? "Customer" : dto.Role
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}
