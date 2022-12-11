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
    private readonly IJwtUtils _jwtUtils;

    public UserService(ApplicationDbContext context, IPasswordHasher passwordHasher, IJwtUtils jwtUtils)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtUtils = jwtUtils;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new NotFoundUserException();
        }

        return user;
    }

    public async Task<AuthenticatedUserDto> SignUp(RegisterDto user, string ipAddress)
    {
        var checkUser = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(user.Email));

        if (checkUser != null)
        {
            throw new EmailAlreadyExistsException();
        }

        user.Password = _passwordHasher.HashPassword(user.Password);

        var jwtToken = _jwtUtils.GenerateJwtToken(user.Email, user.FirstName, user.LastName);
        var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        var newUser = new User
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Password = user.Password,
        };
        newUser.RefreshTokens.Add(refreshToken);
        await _context.Users.AddAsync(newUser);

        // RemoveOldRefreshTokens(newUser);

        await _context.SaveChangesAsync();

        return new AuthenticatedUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = jwtToken,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<AuthenticatedUserDto> SignIn(LoginDto user, string ipAddress)
    {
        var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

        if (dbUser == null || _passwordHasher
                .VerifyHashedPassword(dbUser.Password, user.Password) == PasswordVerificationResult.Failed)
        {
            throw new InvalidEmailPasswordException();
        }

        var jwtToken = _jwtUtils.GenerateJwtToken(dbUser.Email, dbUser.FirstName, dbUser.LastName);
        var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        dbUser.RefreshTokens.Add(refreshToken);

        RemoveOldRefreshTokens(dbUser);

        await _context.SaveChangesAsync();

        return new AuthenticatedUserDto
        {
            Email = dbUser.Email,
            FirstName = dbUser.FirstName,
            LastName = dbUser.LastName,
            AccessToken = jwtToken,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<AuthenticatedUserDto> RefreshToken(string token, string ipAddress)
    {
        var user = GetUserByRefreshToken(token);
        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

        if (refreshToken.IsRevoked)
        {
            // revoke all descendant tokens in case this token has been compromised
            RevokeDescendantRefreshTokens(refreshToken, ipAddress,
                $"Attempted reuse of revoked ancestor token: {token}");
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        if (!refreshToken.IsActive)
            throw new InvalidTokenException();

        // replace old refresh token with a new one (rotate token)
        var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
        user.RefreshTokens.Add(newRefreshToken);

        // remove old refresh tokens from user
        RemoveOldRefreshTokens(user);

        // save changes to db
        _context.Update(user);
        await _context.SaveChangesAsync();

        // generate new jwt
        var jwtToken = _jwtUtils.GenerateJwtToken(user.Email, user.FirstName, user.LastName);

        return new AuthenticatedUserDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AccessToken = jwtToken,
            RefreshToken = newRefreshToken.Token
        };
    }

    public void RevokeToken(string token, string ipAddress)
    {
        var refreshToken = _context.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
            throw new InvalidTokenException();

        // revoke token and save
        RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
        _context.Update(refreshToken);
        _context.SaveChanges();
    }


    // helper methods
    private User GetUserByRefreshToken(string token)
    {
        var user = _context.Users
            .Include("RefreshTokens")
            .SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

        if (user == null)
            throw new InvalidTokenException();

        return user;
    }

    private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
    {
        var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
        return newRefreshToken;
    }

    private void RemoveOldRefreshTokens(User user)
    {
        // remove old inactive refresh tokens from user based on TTL in app settings
        user.RefreshTokens.RemoveAll(x => 
            !x.IsActive && 
            x.Created.AddDays(2) <= DateTime.UtcNow);
    }

    private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, string ipAddress, string reason)
    {
        // recursively traverse the refresh token chain and ensure all descendants are revoked
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            var childToken = _context.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
            if (childToken.IsActive)
                RevokeRefreshToken(childToken, ipAddress, reason);
            else
                RevokeDescendantRefreshTokens(childToken, ipAddress, reason);
        }
    }

    private static void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null,
        string replacedByToken = null)
    {
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = reason;
        token.ReplacedByToken = replacedByToken;
    }
}