using DeliveryReviewAggregator.Models;
using System.Net;
using System.Text.Json;

namespace DeliveryReviewAggregator;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); // Handle exceptions globally
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log the exception (use ILogger for structured logging)
        Console.Error.WriteLine($"Unhandled exception: {exception.Message}");

        var errorResponse = new ApiResponse<object>
        {
            HttpStatusCode = HttpStatusCode.InternalServerError,
            Error = new ApiError
            {
                Message = "An unexpected error occurred.",
                Details = exception.Message
            }
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}