using DeliveryReviewAggregator.Models;

namespace DeliveryReviewAggregator.Services;

public interface IReviewAggregatorService
{
    Task<List<Review>> GetAggregatedReviewsAsync(string placeId);
}