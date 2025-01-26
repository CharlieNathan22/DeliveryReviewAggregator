using DeliveryReviewAggregator.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryReviewAggregator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController(IGooglePlacesService googlePlacesService, IReviewAggregatorService reviewAggregatorService, ILogger<RestaurantsController> logger) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> SearchRestaurants([FromQuery] string location, [FromQuery] int radius = 1500)
    {
        logger.LogInformation("SearchRestaurants endpoint called with location: {Location} & radius: {Radius}", location, radius);

        var response = await googlePlacesService.SearchRestaurantsAsync(location, radius);
        return !response.Success 
            ? StatusCode((int)response.HttpStatusCode, response.Error) 
            : Ok(response.Data);
    }

    [HttpGet("{placeId}/reviews")]
    public async Task<IActionResult> GetAggregatedReviews(string placeId)
    {
        logger.LogInformation("GetAggregatedReviews endpoint called with location: {PlaceId}", placeId);

        var reviews = await reviewAggregatorService.GetAggregatedReviewsAsync(placeId);
        return Ok(reviews);
    }

    [HttpGet("{placeId}/details")]
    public async Task<IActionResult> GetRestaurantDetails(string placeId)
    {
        logger.LogInformation("GetRestaurantDetails endpoint called with location: {PlaceId}", placeId);

        var response = await googlePlacesService.GetPlaceDetailsAsync(placeId);
        return !response.Success 
            ? StatusCode((int)response.HttpStatusCode, response.Error) 
            : Ok(response.Data);
    }
}
