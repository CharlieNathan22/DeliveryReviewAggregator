using System.Net;

namespace DeliveryReviewAggregator.Models;

public class ApiResponse<T>
{
    public HttpStatusCode HttpStatusCode { get; set; }
    public T? Data { get; set; }
    public ApiError? Error { get; set; }
    public bool Success => Error == null;

    public static ApiResponse<T> SuccessResponse(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse<T>
        {
            Data = data,
            HttpStatusCode = statusCode
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, HttpStatusCode statusCode, string? code = null)
    {
        return new ApiResponse<T>
        {
            Error = new ApiError { Message = message, Code = code },
            HttpStatusCode = statusCode
        };
    }
}
