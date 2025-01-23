using DeliveryReviewAggregator.Models;

namespace DeliveryReviewAggregator.Services
{
    public class MockRestaurantService
    {
        private readonly List<Restaurant> restaurants = [
            new Restaurant { Id = 1, Name = "The Italian Place", Location = "London", OverallRating = 4.5f, Cuisine = "Italian" },
            new Restaurant { Id = 2, Name = "Sushi World", Location = "Manchester", OverallRating = 4.7f, Cuisine = "Japanese" },
            new Restaurant { Id = 3, Name = "Burger Joint", Location = "Birmingham", OverallRating = 4.3f, Cuisine = "American" }
        ];

        public IEnumerable<Restaurant> GetAllRestaurants() => restaurants;

        public Restaurant? GetRestaurantById(int id) => restaurants.Find(r => r.Id == id);
    }
}
