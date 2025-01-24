
using DeliveryReviewAggregator.Clients;
using DeliveryReviewAggregator.Services;

namespace DeliveryReviewAggregator;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpClient<IGooglePlacesClient, GooglePlacesClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        builder.Services.AddScoped<IGooglePlacesService, GooglePlacesService>();
        builder.Services.AddScoped<IReviewService, GooglePlacesService>();
        builder.Services.AddScoped<IReviewAggregatorService, ReviewAggregatorService>();

        // Add services to the container.
        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
