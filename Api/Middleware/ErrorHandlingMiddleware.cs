using System.Net;
using System.Text.Json;
using ApplicationCore.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Middleware;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    
public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            

            ProblemDetails problem = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = e.Message,
            };
            
            switch (e)
            {
                case EmailAlreadyExistsException _:
                    problem.Status = (int)HttpStatusCode.Conflict;
                    problem.Title = "Email already exists";
                    break;
                case InvalidEmailPasswordException _:
                    problem.Status = (int)HttpStatusCode.InternalServerError;
                    problem.Title = "Invalid email or password";
                    break;
                case NotFoundTransactionException _:
                    problem.Status = (int)HttpStatusCode.NotFound;
                    problem.Title = "Transaction not found";
                    break;
                case NotFoundAccountException _:
                    problem.Status = (int)HttpStatusCode.NotFound;
                    problem.Title = "Account not found";
                    break;
                case NotFoundDepositException _:
                    problem.Status = (int)HttpStatusCode.NotFound;
                    problem.Title = "Deposit not found";
                    break;
                case NoReceiptOfTransactionException _:
                    problem.Status = (int)HttpStatusCode.NotFound;
                    problem.Title = "Receipt not found";
                    break;
                case NotFoundTransactionReceiptException _:
                    problem.Status = (int)HttpStatusCode.NotFound;
                    problem.Title = "Transaction does not have a receipt";
                    break;
                case InvalidTokenException _:
                    problem.Status = (int)HttpStatusCode.Unauthorized;
                    problem.Title = "Invalid token";
                    break;
            }

            string json = JsonSerializer.Serialize(problem);
            
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(json);
            
        }
    }
}