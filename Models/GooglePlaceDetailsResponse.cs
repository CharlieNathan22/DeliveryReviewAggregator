namespace DeliveryReviewAggregator.Models;

public class GooglePlaceDetailsResponse
{
    public PlaceDetailsResult? Result { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; } // Google API error message
    public List<string> HtmlAttributions { get; set; } = [];
}
