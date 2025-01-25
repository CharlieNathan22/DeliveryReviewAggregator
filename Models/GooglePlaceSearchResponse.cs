namespace DeliveryReviewAggregator.Models;

public class GooglePlaceSearchResponse
{
    public List<PlaceSearchResult> Results { get; set; } = [];
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public string? NextPageToken { get; set; }
    public List<string> HtmlAttributions { get; set; } = [];
}
