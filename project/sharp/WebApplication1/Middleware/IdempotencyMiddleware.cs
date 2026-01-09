using System.Text;
using WebApplication1.Services;

namespace WebApplication1.Middleware;

public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _logger;

    public IdempotencyMiddleware(
        RequestDelegate next,
        ILogger<IdempotencyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IdempotencyService idempotencyService)
    {
        // Only apply to POST requests
        if (context.Request.Method != "POST")
        {
            await _next(context);
            return;
        }

        var idempotencyKey = context.Request.Headers["Idempotency-Key"].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            await _next(context);
            return;
        }

        // Check if we have a cached response
        var cachedResponse = await idempotencyService.GetCachedResponseAsync(idempotencyKey);
        if (cachedResponse != null)
        {
            _logger.LogInformation("Returning cached response for idempotency key {Key}", idempotencyKey);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(cachedResponse);
            return;
        }

        // Read request body
        context.Request.EnableBuffering();
        var bodyStream = context.Request.Body;
        var reader = new StreamReader(bodyStream, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        bodyStream.Position = 0;

        // Store original response body stream
        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            // If successful (2xx), cache the response
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                responseBody.Position = 0;
                var responseBodyText = await new StreamReader(responseBody).ReadToEndAsync();
                
                await idempotencyService.CacheResponseAsync(
                    idempotencyKey,
                    responseBodyText,
                    TimeSpan.FromHours(24));

                responseBody.Position = 0;
                await responseBody.CopyToAsync(originalBodyStream);
            }
            else
            {
                responseBody.Position = 0;
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}

