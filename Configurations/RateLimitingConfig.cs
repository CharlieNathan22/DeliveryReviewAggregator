namespace DeliveryReviewAggregator.Configurations;

public class RateLimitingConfig
{
    public const string Section = "RateLimiting";

    public FixedWindowRateLimiterConfig? FixedWindow { get; set; }
    public ConcurrencyLimiterConfig? ConcurrencyLimit { get; set; }
}

public class FixedWindowRateLimiterConfig
{
    public int PermitLimit { get; set; }
    public int WindowSeconds { get; set; }
    public int QueueLimit { get; set; }
}

public class ConcurrencyLimiterConfig
{
    public int PermitLimit { get; set; }
    public int QueueLimit { get; set; }
}