using DeliveryReviewAggregator.Clients;
using DeliveryReviewAggregator.Models;

namespace DeliveryReviewAggregator.Services;
public class GooglePlacesService(IGooglePlacesClient googlePlacesClient) : IGooglePlacesService, IReviewService
{
    public async Task<ApiResponse<List<PlaceTextSearchResult>>> SearchRestaurantsAsync(string location, int radius = 1500)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return new ApiResponse<List<PlaceTextSearchResult>>
            {
                Success = false,
                ErrorMessage = "Location cannot be empty"
            };
        }

        var response = await googlePlacesClient.SearchRestaurantsAsync(location, radius);
        if (!response.Success)
        {
            return new ApiResponse<List<PlaceTextSearchResult>>
            {
                Success = false,
                ErrorMessage = response.ErrorMessage
            };
        }

        return new ApiResponse<List<PlaceTextSearchResult>>
        {
            Success = true,
            Data = response.Data?.Results
        };
    }

    public async Task<ApiResponse<PlaceDetailsResult>> GetPlaceDetailsAsync(string placeId)
    {
        if (string.IsNullOrWhiteSpace(placeId))
        {
            return new ApiResponse<PlaceDetailsResult>
            {
                Success = false,
                ErrorMessage = "PlaceId cannot be empty"
            };
        }

        var response = await googlePlacesClient.GetPlaceDetailsAsync(placeId);
        if (!response.Success)
        {
            return new ApiResponse<PlaceDetailsResult>
            {
                Success = false,
                ErrorMessage = response.ErrorMessage
            };
        }

        return new ApiResponse<PlaceDetailsResult>
        {
            Success = true,
            Data = response.Data?.Result
        };
    }

    public async Task<List<Review>> GetReviewsAsync(string placeId)
    {
        if (string.IsNullOrWhiteSpace(placeId))
        {
            throw new ArgumentException("PlaceId cannot be empty", nameof(placeId));
        }

        var response = await googlePlacesClient.GetPlaceDetailsAsync(placeId);
        if (!response.Success || response.Data?.Result?.Reviews == null)
        {
            return [];
        }

        return response.Data.Result.Reviews;
    }
}

