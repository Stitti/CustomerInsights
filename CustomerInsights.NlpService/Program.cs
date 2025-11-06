using CustomerInsights.NlpService.Runtime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using CustomerInsights.ServiceDefaults;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.AddServiceDefaults();

builder.Services.AddScoped<ZeroShotAspectNliOnnx>();
builder.Services.AddScoped<SentimentOnnx3>();
builder.Services.AddScoped<EmotionOnnxMulti>();
builder.Services.AddScoped<UrgencyOnnx3>();
builder.Services.AddScoped<TextAnalyzer>();

builder.AddLogging();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Internal API", Version = "v1" });
    //o.AddServer(new OpenApiServer { Url = "http://localhost:5200" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

try
{
    Log.Information("Starting application...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application terminated unexpectedly.");
    throw;
}
finally
{
    Log.Information("Application is shutting down...");
    Log.CloseAndFlush();
}