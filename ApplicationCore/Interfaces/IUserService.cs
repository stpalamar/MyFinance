using ApplicationCore.DTO;
using Infrastructure.Data.Models;

namespace ApplicationCore.Interfaces;

public interface IUserService
{
    Task<User> GetUserByEmail(string email);
    Task<AuthenticatedUserDto> SignUp(RegisterDto user, string ipAddress);
    Task<AuthenticatedUserDto> SignIn(LoginDto user, string ipAddress);
    Task<AuthenticatedUserDto> RefreshToken(string token, string ipAddress);
    
}