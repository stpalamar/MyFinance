using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Infrastructure.Data.Models;
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
    public async Task<IActionResult> SignUp(User user)
    {
        var result = await _userService.SignUp(user);
        return Created("", result);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(LoginDto user)
    {
        var result = await _userService.SignIn(user);
        return Ok(result);
    }
}