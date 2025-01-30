using DeliveryReviewAggregator.Configurations;
using System.Threading.RateLimiting;

namespace DeliveryReviewAggregator.Factories;

public class RateLimiterFactory
{
    public static PartitionedRateLimiter<HttpContext> CreateFixedWindowLimiter(RateLimitingConfig config)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
                new FixedWindowRateLimiterOptions
                {
                    PermitLimit = config.FixedWindow!.PermitLimit,
                    Window = TimeSpan.FromSeconds(config.FixedWindow.WindowSeconds),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = config.FixedWindow.QueueLimit
                });
        });
    }

    public static PartitionedRateLimiter<HttpContext> CreateConcurrencyLimiter(RateLimitingConfig config)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            return RateLimitPartition.GetConcurrencyLimiter(partitionKey, _ =>
                new ConcurrencyLimiterOptions
                {
                    PermitLimit = config.ConcurrencyLimit!.PermitLimit,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = config.ConcurrencyLimit.QueueLimit
                });
        });
    }
}