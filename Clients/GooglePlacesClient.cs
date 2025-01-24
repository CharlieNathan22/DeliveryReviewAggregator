using DeliveryReviewAggregator.Models;
using System.Net.Http;

namespace DeliveryReviewAggregator.Clients;

public class GooglePlacesClient(HttpClient httpClient) : IGooglePlacesClient
{
    private readonly string apiKey = "AIzaSyBTOBvLtPe5Zh2GZsqFgGC73pArBFWHwqw";

    public async Task<ApiResponse<GooglePlaceSearchResponse>> SearchRestaurantsAsync(string location, int radius = 1500, string type = "restaurant")
    {
        var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={type}+in+{location}&radius={radius}&key={apiKey}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<GooglePlaceSearchResponse>
            {
                Success = false,
                ErrorMessage = response.ReasonPhrase
            };
        }

        var result = await response.Content.ReadFromJsonAsync<GooglePlaceSearchResponse>();
        return result != null
            ? new ApiResponse<GooglePlaceSearchResponse> { Success = true, Data = result }
            : new ApiResponse<GooglePlaceSearchResponse> { Success = false, ErrorMessage = "Failed to deserialize response" };
    }

    public async Task<ApiResponse<GooglePlaceDetailsResponse>> GetPlaceDetailsAsync(string placeId)
    {
        var url = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={placeId}&key={apiKey}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<GooglePlaceDetailsResponse>
            {
                Success = false,
                ErrorMessage = response.ReasonPhrase
            };
        }

        var result = await response.Content.ReadFromJsonAsync<GooglePlaceDetailsResponse>();
        return result != null
            ? new ApiResponse<GooglePlaceDetailsResponse> { Success = true, Data = result }
            : new ApiResponse<GooglePlaceDetailsResponse> { Success = false, ErrorMessage = "Failed to deserialize response" };
    }
}
