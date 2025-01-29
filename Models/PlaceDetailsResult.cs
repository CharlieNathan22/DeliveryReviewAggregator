namespace DeliveryReviewAggregator.Models;

public class PlaceDetailsResult
{
    public string Name { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Address { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public List<Review> Reviews { get; set; } = [];
    public List<string> Photos { get; set; } = [];
}