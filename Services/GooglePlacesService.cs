using DeliveryReviewAggregator.Models;
using Newtonsoft.Json;

namespace DeliveryReviewAggregator.Services
{
    public class GooglePlacesService(HttpClient httpClient)
    {
        private readonly string apiKey = "AIzaSyBTOBvLtPe5Zh2GZsqFgGC73pArBFWHwqw";

        public async Task<ApiResponse<GooglePlaceDetailsResponse>> GetReviewsAsync(string placeId)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={placeId}&key={apiKey}";
            var httpResponse = await httpClient.GetAsync(url);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return new ApiResponse<GooglePlaceDetailsResponse>()
                {
                    Success = false,
                    ErrorMessage = httpResponse.ReasonPhrase
                };
            }
            var result = await httpResponse.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<GooglePlaceDetailsResponse>(result);

            if (data == null) {
                return new ApiResponse<GooglePlaceDetailsResponse>
                {
                    Success = false,
                    ErrorMessage = httpResponse.ReasonPhrase
                };
            }

            return new ApiResponse<GooglePlaceDetailsResponse>
            { 
                Success = true,
                Data = data

            };            
        }
    }
}
