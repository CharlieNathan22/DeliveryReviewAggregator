namespace DeliveryReviewAggregator.Models;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public float OverallRating { get; set; }
    public string Cuisine { get; set; } = string.Empty;
    public List<Review> GoogleReviews { get; set; } = [];
}
