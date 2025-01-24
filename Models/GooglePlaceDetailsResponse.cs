namespace DeliveryReviewAggregator.Models;

public class GooglePlaceDetailsResponse
{
    public PlaceDetailsResult? Result { get; set; }
    public string Status { get; set; } = string.Empty;
}
