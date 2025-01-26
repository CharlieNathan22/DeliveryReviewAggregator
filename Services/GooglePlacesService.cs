using DeliveryReviewAggregator.Clients;
using DeliveryReviewAggregator.Models;
using System.Net;

namespace DeliveryReviewAggregator.Services;
public class GooglePlacesService(IGooglePlacesClient googlePlacesClient) : IGooglePlacesService, IReviewService
{
    public async Task<ApiResponse<List<PlaceSearchResult>>> SearchRestaurantsAsync(string location, int radius = 1500)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return ApiResponse<List<PlaceSearchResult>>.ErrorResponse(
                "Location cannot be empty.",
                HttpStatusCode.BadRequest,
                "INVALID_REQUEST"
            );
        }

        var response = await googlePlacesClient.SearchRestaurantsAsync(location, radius);

        if (!response.Success)
        {
            return ApiResponse<List<PlaceSearchResult>>.ErrorResponse(
                 response.Error!.Message,
                 response.HttpStatusCode,
                 response.Error!.Code
             );
        }

        return ApiResponse<List<PlaceSearchResult>>.SuccessResponse(response.Data!.Results);

    }

    public async Task<ApiResponse<PlaceDetailsResult>> GetPlaceDetailsAsync(string placeId)
    {
        if (string.IsNullOrWhiteSpace(placeId))
        {
            return ApiResponse<PlaceDetailsResult>.ErrorResponse(
                "PlaceId cannot be empty.",
                HttpStatusCode.BadRequest,
                "INVALID_REQUEST"
            );
        }

        var response = await googlePlacesClient.GetPlaceDetailsAsync(placeId);

        if (!response.Success)
        {
            return ApiResponse<PlaceDetailsResult>.ErrorResponse(
                response.Error!.Message,
                response.HttpStatusCode,
                response.Error!.Code
            );
        }

        return response.Data?.Result == null 
            ? ApiResponse<PlaceDetailsResult>.ErrorResponse("Place details not found.", HttpStatusCode.NotFound, "NOT_FOUND") 
            : ApiResponse<PlaceDetailsResult>.SuccessResponse(response.Data!.Result!);
    }

    public async Task<List<Review>> GetReviewsAsync(string placeId)
    {
        var detailsResponse = await GetPlaceDetailsAsync(placeId);
        return detailsResponse is { Success: true, Data.Reviews: not null }
            ? detailsResponse.Data.Reviews
            : [];
    }
}

