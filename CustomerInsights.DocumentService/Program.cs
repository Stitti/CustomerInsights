using CustomerInsights.DocumentService.Services;
using CustomerInsights.ServiceDefaults;
using Microsoft.OpenApi;
using Serilog;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();

        string tessdataPath = Path.Combine(builder.Environment.ContentRootPath, "tessdata");

        builder.Services.AddSingleton(sp =>
            new PdfTextExtractionService(
                tessdataPath: tessdataPath,
                language: "deu+eng",   // Sprachpakete im tessdata-Ordner
                dpi: 300,
                ocrTriggerMinChars: 64)
        );
        builder.Services.AddSingleton<EmlReaderService>();
        builder.Services.AddSingleton<ZugferdExtractionService>();

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

        app.UseCors("AllowAll");
        app.MapControllers();

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
    }
}