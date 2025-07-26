using System.Reflection;
using Microsoft.EntityFrameworkCore;
using N5Challenge.Domain;
using N5Challenge.Enrichers;
using N5Challenge.Repositories;
using N5Challenge.Repositories.Interfaces;
using N5Challenge.Services;
using N5Challenge.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog((provider, configuration) =>
{
    configuration.ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(provider)
        .Enrich.FromLogContext()
        .Enrich.With<ClassNameEnricher>();
});
builder.Services.AddDbContext<N5DbContext>(optionsBuilder =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    optionsBuilder.UseSqlServer(cs);
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

Log.Information("Component has started and it's ready");

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
