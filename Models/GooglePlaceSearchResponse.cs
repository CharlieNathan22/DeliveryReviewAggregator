namespace DeliveryReviewAggregator.Models
{
    public class GooglePlaceSearchResponse
    {
        public List<PlaceTextSearchResult> Results { get; set; } = [];
        public string Status { get; set; } = string.Empty;
    }
}
