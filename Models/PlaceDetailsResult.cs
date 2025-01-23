using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace DeliveryReviewAggregator.Models
{
    public class PlaceDetailsResult
    {
        public string Name { get; set; } = string.Empty;
        public int Rating { get; set; }
        public List<Review> Reviews { get; set; } = [];
    }
}
