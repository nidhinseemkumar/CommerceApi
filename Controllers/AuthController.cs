using CommerceApi.Services;
using CommerceApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommerceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CommerceApi.DTOs.RegisterDto dto)
    {
        try
        {
            await authService.RegisterAsync(dto);
            return Ok("Registered successfully.");
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Unexpected error: " + ex.Message);
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] CommerceApi.DTOs.LoginDto dto)
    {
        try
        {
            var token = authService.Login(dto);
            return Ok(new { token });
        }
        catch (Exceptions.CommerceException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Unexpected error: " + ex.Message);
        }
    }
}