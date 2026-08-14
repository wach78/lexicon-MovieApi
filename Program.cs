using Microsoft.EntityFrameworkCore;
using MovieApi.Data.Seed;
using MovieApi.Interfaces.Data;
using MovieApi.Interfaces.Service;
using MovieApi.Services;
using Scalar.AspNetCore;
namespace MovieApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("MovieApiContext") ?? throw new InvalidOperationException("Connection string 'MovieApiContext' not found.");

        builder.Services.AddDbContext<MovieApiContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddScoped<IMovieApiContext>(serviceProvider => serviceProvider.GetRequiredService<MovieApiContext>());
        builder.Services.AddScoped<IMovieService, MovieService>();
        builder.Services.AddScoped<IReviewService, ReviewService>();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy
                    .WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:5174")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.SeedData();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        app.UseCors("Frontend");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
