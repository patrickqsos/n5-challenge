using System.Reflection;
using Destructurama;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Microsoft.EntityFrameworkCore;
using N5Challenge.Constants;
using N5Challenge.Domain;
using N5Challenge.Enrichers;
using N5Challenge.Middlewares;
using N5Challenge.Repositories;
using N5Challenge.Repositories.Interfaces;
using N5Challenge.Services;
using N5Challenge.Services.Interfaces;
using Serilog;
//using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog((provider, configuration) =>
{
    var elasticsearchConfig = builder.Configuration.GetSection("Elasticsearch");
    /*
    var esConfig = new ElasticsearchSinkOptions(new Uri(elasticsearchConfig.GetValue<string>("host")))
    {
        BatchAction = ElasticOpType.Create,
        
        ModifyConnectionSettings = connectionConfiguration =>
        {
            connectionConfiguration.EnableHttpCompression();
            connectionConfiguration.BasicAuthentication();
            return connectionConfiguration;
        },
        IndexDecider = (logEvent, _) => logEvent.Properties.TryGetValue(SerilogConstants.LogType, out var logType) ? logType.ToString().ToLower() : "general"
    };
    */
    
    Log.Information("Configuring serilog sink for elasticsearch with host: {elasticSearchHost}", elasticsearchConfig.GetValue<string>("host"));

    configuration.ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(provider)
        .Destructure.JsonNetTypes()
        //.Destructure.UsingAttributes()
        .Enrich.FromLogContext()
        .Enrich.WithProperty(SerilogConstants.LogType, SerilogConstants.LogGeneral)
        .Enrich.With<ClassNameEnricher>()
        .WriteTo.Elasticsearch([new Uri(elasticsearchConfig.GetValue<string>("host") ?? throw new InvalidOperationException())], opts =>
        {
            opts.DataStream = new DataStreamName("logs", "registry", "n5");
            opts.BootstrapMethod = BootstrapMethod.Failure;
            
        }, transport =>
        {
            transport.Authentication(new BasicAuthentication(
                elasticsearchConfig.GetValue<string>("user") ?? throw new InvalidOperationException(), 
                elasticsearchConfig.GetValue<string>("password") ?? throw new InvalidOperationException()
                ));
        });
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

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    Log.Information("Applying DB migrations");

    var dbContext = scope.ServiceProvider.GetRequiredService<N5DbContext>();
    dbContext.Database.Migrate();
}

Log.Information("Component has started with configured env: {environment}", builder.Environment.EnvironmentName);

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();

// Public class for integration tests
public partial class Program { }
