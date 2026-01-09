using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models.DTO;

namespace WebApplication1.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred processing request {TraceId}. Error: {ErrorMessage}", 
                context.TraceIdentifier, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var error = "InternalServerError";
        var message = "An error occurred while processing your request.";

        switch (exception)
        {
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                error = "NotFound";
                message = exception.Message;
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                error = "Unauthorized";
                message = exception.Message;
                break;
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                error = "BadRequest";
                message = exception.Message;
                break;
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                error = "BadRequest";
                message = exception.Message;
                break;
            case DbUpdateException dbEx:
                statusCode = HttpStatusCode.InternalServerError;
                error = "DatabaseError";
                message = "A database error occurred.";
                if (_environment.IsDevelopment())
                {
                    message += $" {dbEx.InnerException?.Message ?? dbEx.Message}";
                }
                _logger.LogError(dbEx, "Database error: {Error}", dbEx.Message);
                break;
            case Npgsql.PostgresException pgEx:
                statusCode = HttpStatusCode.InternalServerError;
                error = "DatabaseError";
                message = "A database error occurred.";
                if (_environment.IsDevelopment())
                {
                    message += $" {pgEx.Message}";
                }
                _logger.LogError(pgEx, "PostgreSQL error: {Error}", pgEx.Message);
                break;
        }

        // In development, include more details
        if (_environment.IsDevelopment() && statusCode == HttpStatusCode.InternalServerError)
        {
            message = $"{message} Details: {exception.Message}";
            if (exception.InnerException != null)
            {
                message += $" Inner: {exception.InnerException.Message}";
            }
        }

        var response = new ErrorDto
        {
            Error = error,
            Message = message,
            TraceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

