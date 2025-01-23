using DeliveryReviewAggregator.Models;
using DeliveryReviewAggregator.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryReviewAggregator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController(MockRestaurantService restaurantService, GooglePlacesService googlePlacesService) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllRestaurants()
        {
            var restaurants = restaurantService.GetAllRestaurants();
            return Ok(restaurants);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurantReviewsById(int id)
        {
            var restaurant = restaurantService.GetRestaurantById(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            var googleReviews = await googlePlacesService.GetReviewsAsync("YOUR_GOOGLE_PLACE_ID");
            if (googleReviews.Success)
            {
                restaurant.GoogleReviews = googleReviews.Data?.Result?.Reviews ?? [];
            }

            return Ok(restaurant);
        }
    }
}
