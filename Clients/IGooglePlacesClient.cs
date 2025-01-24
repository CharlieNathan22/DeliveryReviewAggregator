using DeliveryReviewAggregator.Models;

namespace DeliveryReviewAggregator.Clients;

public interface IGooglePlacesClient
{
    Task<ApiResponse<GooglePlaceSearchResponse>> SearchRestaurantsAsync(string location, int radius = 1500, string type = "restaurant");
    Task<ApiResponse<GooglePlaceDetailsResponse>> GetPlaceDetailsAsync(string placeId);
}
