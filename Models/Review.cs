namespace DeliveryReviewAggregator.Models;

public class Review
{
    public string AuthorName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public double Rating { get; set; }
}