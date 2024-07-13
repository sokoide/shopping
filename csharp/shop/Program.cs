using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

// Globals
Globals.Init();

// Configure Serilog & OTLP logging
Log.Logger = new LoggerConfiguration()
.WriteTo.Console()
.WriteTo.OpenTelemetry(options =>
{
    options.Endpoint = Consts.OTLP_HTTP_ENDPOINT + "v1/logs";
    options.Protocol = OtlpProtocol.HttpProtobuf;
    options.ResourceAttributes = new Dictionary<string, object>
    {
        ["service.name"] = Consts.OTEL_SERVICE_NAME,
    };
})
// .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

Log.Information("Starting up");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS services to the DI container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
                      builder => builder
                      .WithOrigins(Consts.CORS_ORIGINS)
                      .AllowAnyHeader()
                      .AllowAnyMethod());
});

// Configure OTLP Tracing and Metrics
var otel = builder.Services.AddOpenTelemetry()
           .UseOtlpExporter(
               OtlpExportProtocol.Grpc,
               new Uri(Consts.OTLP_GRPC_ENDPOINT));

// Configure OpenTelemetry Resources with the application name
otel.ConfigureResource(resource => resource
                       .AddService(serviceName: builder.Environment.ApplicationName));

// Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
otel.WithMetrics(metrics => metrics
                 // Metrics provider from OpenTelemetry
                 .AddAspNetCoreInstrumentation()
                 // .AddMeter(greeterMeter.Name)
                 // Metrics provides by ASP.NET Core in .NET 8
                 .AddMeter("Microsoft.AspNetCore.Hosting")
                 .AddMeter("Microsoft.AspNetCore.Server.Kestrel"));
// .AddPrometheusExporter());

// Add Tracing for ASP.NET Core and our custom ActivitySource and export
otel.WithTracing(tracing => tracing
                 .AddAspNetCoreInstrumentation()
                 .AddHttpClientInstrumentation()
                 .AddSource(Consts.OTEL_SERVICE_NAME));

var app = builder.Build();

// Use the CORS middleware
app.UseCors("AllowSpecificOrigin");

// Enable static file serving
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "static")),
    RequestPath = "/static"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// don't redirect http -> https
// app.UseHttpsRedirection();

Services services = new Services();
services.Run(app);

Log.CloseAndFlush();
