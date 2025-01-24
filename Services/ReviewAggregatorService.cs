using DeliveryReviewAggregator.Models;

namespace DeliveryReviewAggregator.Services;

public class ReviewAggregatorService(IEnumerable<IReviewService> reviewServices) : IReviewAggregatorService
{
    public async Task<List<Review>> GetAggregatedReviewsAsync(string placeId)
    {
        var tasks = reviewServices.Select(service => service.GetReviewsAsync(placeId));
        var results = await Task.WhenAll(tasks);

        return results.SelectMany(r => r).ToList(); // Combine all reviews
    }
}