using CommerceApi.Models;
using CommerceApi.Data;

namespace CommerceApi.Services;

public partial class AuthService(AppDbContext db, IConfiguration config) : IAuthService
{
}
