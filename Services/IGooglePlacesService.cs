using DeliveryReviewAggregator.Models;

namespace DeliveryReviewAggregator.Services;

public interface IGooglePlacesService
{
    Task<ApiResponse<List<PlaceSearchResult>>> SearchRestaurantsAsync(string location, int radius = 10);
    Task<ApiResponse<PlaceDetailsResult>> GetPlaceDetailsAsync(string placeId);
}
