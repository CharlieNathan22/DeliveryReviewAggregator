namespace DeliveryReviewAggregator.Models;

public class ApiError
{
    public string Message { get; set; } = string.Empty;
    public string? Code { get; set; }
}
