using DeliveryReviewAggregator.Clients;
using DeliveryReviewAggregator.Configurations;
using DeliveryReviewAggregator.Middleware;
using DeliveryReviewAggregator.Services;
using Serilog;
using System.Reflection;

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

        builder.Services.Configure<GooglePlacesSettings>(builder.Configuration.GetSection(GooglePlacesSettings.Section));

        builder.Services.AddHttpClient<IGooglePlacesClient, GooglePlacesClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        builder.Services.AddScoped<IGooglePlacesService, GooglePlacesService>();
        builder.Services.AddScoped<IReviewService, GooglePlacesService>();
        builder.Services.AddScoped<IReviewAggregatorService, ReviewAggregatorService>();

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        var app = builder.Build();

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
