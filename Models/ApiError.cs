namespace DeliveryReviewAggregator.Models;

public class ApiError
{
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? Code { get; set; }
}