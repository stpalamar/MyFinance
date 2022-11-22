using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using ApplicationCore.Utilities;
using Infrastructure;
using Infrastructure.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    
    public UserService(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }
  
    public async Task<AuthenticatedUserDto> SignUp(User user)
    {
        var checkUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(user.Email));
        
        if (checkUser != null)
        {
            throw new EmailAlreadyExistsException();
        }
        
        user.Password = _passwordHasher.HashPassword(user.Password);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new AuthenticatedUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = JwtGenerator.GenerateUserToken(user.Email, user.FirstName, user.LastName)
        };
    }

    public async Task<AuthenticatedUserDto> SignIn(LoginDto user)
    {
        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        
        if (dbUser == null || _passwordHasher
                .VerifyHashedPassword(dbUser.Password, user.Password) == PasswordVerificationResult.Failed)
        {
            throw new InvalidEmailPasswordException();
        }
        
        return new AuthenticatedUserDto
        {
            Email = dbUser.Email,
            FirstName = dbUser.FirstName,
            LastName = dbUser.LastName,
            Token = JwtGenerator.GenerateUserToken(dbUser.Email, dbUser.FirstName, dbUser.LastName)
        };
    }
}