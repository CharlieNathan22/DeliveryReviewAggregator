using DeliveryReviewAggregator.Clients;
using DeliveryReviewAggregator.Configurations;
using DeliveryReviewAggregator.Factories;
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

        var rateLimitingConfig = builder.Configuration.GetSection(RateLimitingConfig.Section).Get<RateLimitingConfig>()
                                 ?? throw new InvalidOperationException("Rate limiting configuration is missing from appsettings");
        builder.Services.AddRateLimiter(options =>
        {
            options.OnRejected = async (context, _) =>
            {
                Log.Warning("Rate limit exceeded. IP: {IP}, Endpoint: {Endpoint}", context.HttpContext.Connection.RemoteIpAddress, context.HttpContext.Request.Path);

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "Too many requests. Please try again later.",
                    RetryAfter = retryAfter.TotalSeconds
                };

                await context.HttpContext.Response.WriteAsJsonAsync(response, context.HttpContext.RequestAborted);
            };

            // Chained limiter combining Fixed Window and Concurrency
            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                RateLimiterFactory.CreateFixedWindowLimiter(rateLimitingConfig),
                RateLimiterFactory.CreateConcurrencyLimiter(rateLimitingConfig)
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

        app.UseSerilogRequestLogging();
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