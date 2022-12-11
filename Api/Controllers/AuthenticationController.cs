using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthenticationController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(RegisterDto user)
    {
        var response = await _userService.SignUp(user, ipAddress());
        SetTokenCookie(response.RefreshToken);
        response.RefreshToken = null;
        return Ok(response);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(LoginDto user)
    {
        var response = await _userService.SignIn(user, ipAddress());
        SetTokenCookie(response.RefreshToken);
        response.RefreshToken = null;
        return Ok(response);
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _userService.RefreshToken(refreshToken!, ipAddress());
        SetTokenCookie(response.RefreshToken!);
        response.RefreshToken = null;
        return Ok(response);
    }

    // helper methods
    private string ipAddress()
    {
        // get source ip address for the current request
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
    
    
    private void SetTokenCookie(string token)
    {
        // append cookie with refresh token to the http response
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            IsEssential = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}