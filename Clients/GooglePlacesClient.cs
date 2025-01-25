using DeliveryReviewAggregator.Configurations;
using DeliveryReviewAggregator.Models;
using Microsoft.Extensions.Options;
using System.Net;

namespace DeliveryReviewAggregator.Clients;

public class GooglePlacesClient : IGooglePlacesClient
{
    private readonly HttpClient httpClient;
    private readonly string baseUrl;
    private readonly string apiKey;

    public GooglePlacesClient(HttpClient httpClient, IOptions<GooglePlacesSettings> settings)
    {
        this.httpClient = httpClient;
        baseUrl = settings.Value.BaseUrl;
        apiKey = settings.Value.ApiKey;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentNullException(nameof(apiKey), "Google Places API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentNullException(nameof(baseUrl), "Google Places Base URL is not configured.");
        }
    }

    public async Task<ApiResponse<GooglePlaceSearchResponse>> SearchRestaurantsAsync(string location, int radius)
    {
        var url = $"{baseUrl}/textsearch/json?query=food+in+{location}&radius={radius}&type=restaurant&key={apiKey}";

        try
        {
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
        var url = $"{baseUrl}/details/json?placeid={placeId}&key={apiKey}";

        try
        {
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
