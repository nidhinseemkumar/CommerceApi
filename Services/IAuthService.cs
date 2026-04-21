using CommerceApi.Models;
using CommerceApi.DTOs;

namespace CommerceApi.Services;

public interface IAuthService
{
    Task RegisterAsync(CommerceApi.DTOs.RegisterDto dto);
    string Login(CommerceApi.DTOs.LoginDto dto);
}
