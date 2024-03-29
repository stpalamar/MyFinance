﻿using ApplicationCore.DTO;
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
        var response = await _userService.SignUp(user, IpAddress());
        SetTokenCookie(response.RefreshToken!);
        response.RefreshToken = null;
        return Ok(response);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(LoginDto user)
    {
        var response = await _userService.SignIn(user, IpAddress());
        SetTokenCookie(response.RefreshToken!);
        response.RefreshToken = null;
        return Ok(response);
    }
    
    [HttpGet("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var response = await _userService.RefreshToken(refreshToken!, IpAddress());
        SetTokenCookie(response.RefreshToken!);
        response.RefreshToken = null;
        return Ok(response);
    }
    
    [HttpGet("logout")]
    public async Task<IActionResult> RevokeToken()
    {
        // accept refresh token in request body or cookie
        var token = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token is required" });

        await _userService.RevokeToken(token, IpAddress());
        return Ok(new { message = "Token revoked" });
    }

    // helper methods
    private string IpAddress()
    {
        // get source ip address for the current request
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"]!;
        else
            return HttpContext.Connection.RemoteIpAddress!.MapToIPv4().ToString();
    }
    
    
    private void SetTokenCookie(string token)
    {
        // append cookie with refresh token to the http response
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}