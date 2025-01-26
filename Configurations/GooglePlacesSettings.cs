namespace DeliveryReviewAggregator.Configurations;

public class GooglePlacesSettings
{
    public const string Section = "GooglePlaces";
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://maps.googleapis.com/maps/api";
}
