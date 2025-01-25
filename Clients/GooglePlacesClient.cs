using DeliveryReviewAggregator.Models;
using System.Net;
using System.Net.Http;

namespace DeliveryReviewAggregator.Clients;

public class GooglePlacesClient(HttpClient httpClient) : IGooglePlacesClient
{
    private readonly string apiKey = "AIzaSyBTOBvLtPe5Zh2GZsqFgGC73pArBFWHwqw";

    public async Task<ApiResponse<GooglePlaceSearchResponse>> SearchRestaurantsAsync(string location, int radius)
    {
        try
        {
            var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query=food+in+{location}&radius={radius}&type=restaurant&key={apiKey}";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return ApiResponse<GooglePlaceSearchResponse>.ErrorResponse(
                    $"Failed to fetch restaurant data. HTTP {response.StatusCode}",
                    response.StatusCode
                );
            }

            var data = await response.Content.ReadFromJsonAsync<GooglePlaceSearchResponse>();
            if (data == null || data.Status != "OK")
            {
                return ApiResponse<GooglePlaceSearchResponse>.ErrorResponse(
                    data?.ErrorMessage ?? "An unknown error occurred.",
                    HttpStatusCode.BadRequest,
                    data?.Status
                );
            }

            return ApiResponse<GooglePlaceSearchResponse>.SuccessResponse(data);
        }
        catch (Exception ex)
        {
            return ApiResponse<GooglePlaceSearchResponse>.ErrorResponse(
                $"An exception occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ApiResponse<GooglePlaceDetailsResponse>> GetPlaceDetailsAsync(string placeId)
    {
        try
        {
            var url = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={placeId}&key={apiKey}";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return ApiResponse<GooglePlaceDetailsResponse>.ErrorResponse(
                    $"Failed to fetch place details. HTTP {response.StatusCode}",
                    response.StatusCode
                );
            }

            var data = await response.Content.ReadFromJsonAsync<GooglePlaceDetailsResponse>();
            if (data == null || data.Status != "OK")
            {
                return ApiResponse<GooglePlaceDetailsResponse>.ErrorResponse(
                    data?.ErrorMessage ?? "An unknown error occurred.",
                    HttpStatusCode.BadRequest,
                    data?.Status
                );
            }

            return ApiResponse<GooglePlaceDetailsResponse>.SuccessResponse(data);
        }
        catch (Exception ex)
        {
            return ApiResponse<GooglePlaceDetailsResponse>.ErrorResponse(
                $"An exception occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }
}
