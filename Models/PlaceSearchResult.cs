namespace DeliveryReviewAggregator.Models;

public class PlaceSearchResult
{
    public string Name { get; set; } = string.Empty;
    public string PlaceId { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Rating { get; set; }
    public List<string> Photos { get; set; } = [];
}