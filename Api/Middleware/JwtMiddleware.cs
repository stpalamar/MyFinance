using ApplicationCore.Interfaces;
using ApplicationCore.Utilities;

namespace Api.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var email = jwtUtils.ValidateJwtToken(token!);
        if (email != null)
        {
            // attach user to context on successful jwt validation
            context.Items["User"] = await userService.GetUserByEmail(email);
        }

        await _next(context);
    }
}