using DeliveryReviewAggregator.Clients;
using DeliveryReviewAggregator.Configurations;
using DeliveryReviewAggregator.Middleware;
using DeliveryReviewAggregator.Services;
using Serilog;
using System.Globalization;
using System.Reflection;
using System.Threading.RateLimiting;

namespace DeliveryReviewAggregator;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration) // Read settings from appsettings.json
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Host.UseSerilog();

        builder.Services.Configure<GooglePlacesConfig>(builder.Configuration.GetSection(GooglePlacesConfig.Section));
        builder.Services.AddHttpClient<IGooglePlacesClient, GooglePlacesClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        builder.Services.AddScoped<IGooglePlacesService, GooglePlacesService>();
        builder.Services.AddScoped<IReviewService, GooglePlacesService>();
        builder.Services.AddScoped<IReviewAggregatorService, ReviewAggregatorService>();

        // Bind RateLimitingConfig from appsettings.json
        var rateLimitingConfig = builder.Configuration.GetSection("RateLimiting").Get<RateLimitingConfig>();

        // Add Rate Limiting Services
        builder.Services.AddRateLimiter(options =>
        {
            // Custom rejection response
            options.OnRejected = (context, _) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return new ValueTask(context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken: _));
            };

            // Chained limiter combining Fixed Window and Concurrency
            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = rateLimitingConfig!.FixedWindow!.PermitLimit,
                            Window = TimeSpan.FromSeconds(rateLimitingConfig.FixedWindow.WindowSeconds),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = rateLimitingConfig.FixedWindow.QueueLimit
                        });
                }),
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetConcurrencyLimiter(partitionKey, _ =>
                        new ConcurrencyLimiterOptions
                        {
                            PermitLimit = rateLimitingConfig!.ConcurrencyLimit!.PermitLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = rateLimitingConfig.ConcurrencyLimit.QueueLimit
                        });
                })
            );
        });

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

        app.UseRateLimiter();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.MapControllers();
        app.Run();
    }
}