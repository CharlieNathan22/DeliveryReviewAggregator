using System.Net;
using System.Text.Json;
using DeliveryReviewAggregator.Models;
using Serilog;

namespace DeliveryReviewAggregator.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); // Handle exceptions globally
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Log.Error(exception, "An unhandled exception occurred.");

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