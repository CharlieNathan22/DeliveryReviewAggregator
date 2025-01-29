using DeliveryReviewAggregator.Models;

namespace DeliveryReviewAggregator.Services;

public interface IReviewService
{
    Task<List<Review>> GetReviewsAsync(string placeId);
}