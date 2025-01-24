using DeliveryReviewAggregator.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryReviewAggregator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController(IGooglePlacesService googlePlacesService, IReviewAggregatorService reviewAggregatorService) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> SearchRestaurants([FromQuery] string location, [FromQuery] int radius = 1500)
    {
        var response = await googlePlacesService.SearchRestaurantsAsync(location, radius);
        if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Data);
    }

    [HttpGet("{placeId}/reviews")]
    public async Task<IActionResult> GetAggregatedReviews(string placeId)
    {
        var reviews = await reviewAggregatorService.GetAggregatedReviewsAsync(placeId);
        return Ok(reviews);
    }

    [HttpGet("{placeId}/details")]
    public async Task<IActionResult> GetRestaurantDetails(string placeId)
    {
        var response = await googlePlacesService.GetPlaceDetailsAsync(placeId);
        if (!response.Success)
        {
            return BadRequest(response.ErrorMessage);
        }

        return Ok(response.Data);
    }
}
