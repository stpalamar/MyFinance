using ApplicationCore.DTO;
using Infrastructure.Data.Models;

namespace ApplicationCore.Interfaces;

public interface IUserService
{
    Task<AuthenticatedUserDto> SignUp(User user);
    Task<AuthenticatedUserDto> SignIn(LoginDto user);
    
}