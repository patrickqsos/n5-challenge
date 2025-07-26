using Microsoft.EntityFrameworkCore;
using N5Challenge.Domain;
using N5Challenge.Enrichers;
using N5Challenge.Repositories;
using N5Challenge.Repositories.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog((provider, configuration) =>
{
    configuration.ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(provider)
        .Enrich.FromLogContext()
        .Enrich.With<ClassNameEnricher>()
        .WriteTo.Console();
});
builder.Services.AddDbContext<N5DbContext>(optionsBuilder =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    optionsBuilder.UseSqlServer(cs);
});

builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
