using DeliveryReviewAggregator.Configurations;
using DeliveryReviewAggregator.Models;
using Microsoft.Extensions.Options;
using System.Net;

namespace DeliveryReviewAggregator.Clients;

public class GooglePlacesClient : IGooglePlacesClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GooglePlacesClient> _logger;
    private readonly string _baseUrl;
    private readonly string _apiKey;

    public GooglePlacesClient(HttpClient httpClient, IOptions<GooglePlacesSettings> settings, ILogger<GooglePlacesClient> logger)
    {
        _httpClient = httpClient;
        _baseUrl = settings.Value.BaseUrl;
        _apiKey = settings.Value.ApiKey;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            _logger.LogError("GooglePlacesClient initialization failed: API Key is null or empty.");
            throw new ArgumentNullException(nameof(_apiKey), "Google Places API key is not configured.");
        }

        if (string.IsNullOrWhiteSpace(_baseUrl))
        {
            _logger.LogError("GooglePlacesClient initialization failed: Base URL is null or empty.");
            throw new ArgumentNullException(nameof(_baseUrl), "Google Places Base URL is not configured.");
        }
    }

    public async Task<ApiResponse<GooglePlaceSearchResponse>> SearchRestaurantsAsync(string location, int radius)
    {
        var url = $"{_baseUrl}/textsearch/json?query=food+in+{location}&radius={radius}&type=restaurant&key={_apiKey}";
        _logger.LogInformation("SearchRestaurantsAsync called with location: {Location}, radius: {Radius}", location, radius);

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("SearchRestaurantsAsync failed: HTTP {StatusCode} for URL: {Url}", response.StatusCode, url);
                return ApiResponse<GooglePlaceSearchResponse>.ErrorResponse(
                    $"Failed to fetch restaurant data. HTTP {response.StatusCode}",
                    response.StatusCode
                );
            }

            var data = await response.Content.ReadFromJsonAsync<GooglePlaceSearchResponse>();
            if (data is not { Status: "OK" })
            {
                _logger.LogWarning(
                    "GooglePlaceDetails API returned non-OK status: {Status}. Error Message: {ErrorMessage}",
                    data?.Status, data?.ErrorMessage);
                return ApiResponse<GooglePlaceSearchResponse>.ErrorResponse(
                    data?.ErrorMessage ?? "An unknown error occurred.",
                    HttpStatusCode.BadRequest,
                    data?.Status
                );
            }

            _logger.LogInformation("SearchRestaurantsAsync succeeded: Retrieved {ResultCount} results for location: {Location}",
                data.Results.Count, location);

            return ApiResponse<GooglePlaceSearchResponse>.SuccessResponse(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in SearchRestaurantsAsync() GooglePlacesClient for location: {Location}, radius: {Radius}", location, radius);
            return ApiResponse<GooglePlaceSearchResponse>.ErrorResponse(
                $"An exception occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }

    public async Task<ApiResponse<GooglePlaceDetailsResponse>> GetPlaceDetailsAsync(string placeId)
    {
        var url = $"{_baseUrl}/details/json?placeid={placeId}&key={_apiKey}";
        _logger.LogInformation("GetPlaceDetailsAsync called with PlaceId: {PlaceId}", placeId);

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GetPlaceDetailsAsync failed: HTTP {StatusCode} for URL: {Url}", response.StatusCode, url);
                return ApiResponse<GooglePlaceDetailsResponse>.ErrorResponse(
                    $"Failed to fetch place details. HTTP {response.StatusCode}",
                    response.StatusCode
                );
            }

            var data = await response.Content.ReadFromJsonAsync<GooglePlaceDetailsResponse>();
            if (data is not { Status: "OK" })
            {
                _logger.LogWarning("GooglePlaceTextSearch API returned non-OK status: {Status}. Error Message: {ErrorMessage}",
                    data?.Status, data?.ErrorMessage);
                return ApiResponse<GooglePlaceDetailsResponse>.ErrorResponse(
                    data?.ErrorMessage ?? "An unknown error occurred.",
                    HttpStatusCode.BadRequest,
                    data?.Status
                );
            }

            _logger.LogInformation("GetPlaceDetailsAsync succeeded for PlaceId: {PlaceId}", placeId);

            return ApiResponse<GooglePlaceDetailsResponse>.SuccessResponse(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred in GetPlaceDetailsAsync for PlaceId: {PlaceId}", placeId);
            return ApiResponse<GooglePlaceDetailsResponse>.ErrorResponse(
                $"An exception occurred: {ex.Message}",
                HttpStatusCode.InternalServerError
            );
        }
    }
}
