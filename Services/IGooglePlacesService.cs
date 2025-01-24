using DeliveryReviewAggregator.Models;

namespace DeliveryReviewAggregator.Services;

public interface IGooglePlacesService
{
    Task<ApiResponse<List<PlaceTextSearchResult>>> SearchRestaurantsAsync(string location, int radius = 1500);
    Task<ApiResponse<PlaceDetailsResult>> GetPlaceDetailsAsync(string placeId);
}
