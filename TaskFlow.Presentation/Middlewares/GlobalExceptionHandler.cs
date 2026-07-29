using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Exceptions;

namespace TaskFlow.Presentation.Middlewares;
public class GlobalExceptionHandler(
    RequestDelegate next,
    ILogger<GlobalExceptionHandler> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = (int)HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred. Please try again later.";

        switch (exception)
        {
            case AccessDeniedException accessDeniedException:
                statusCode = (int)HttpStatusCode.Forbidden; 
                message = accessDeniedException.Message;
                break;

            case ConflictException conflictException:
                statusCode = (int)HttpStatusCode.Conflict;
                message = conflictException.Message;
                break;

            case NotFoundException notFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                message = notFoundException.Message;
                break;

            case UnauthorizedException unauthorizedException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = unauthorizedException.Message;
                break;

            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = validationException.Message;
                break;

            case Exception genericExeption:
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = genericExeption.Message;
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.StatusCode = statusCode;

        var response = new
        {
            StatusCode = statusCode,
            Message = message
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}
