using CustomerInsights.EmbeddingService.Services;
using CustomerInsights.NlpRuntime;

WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.Services.AddControllers();
webApplicationBuilder.Services.AddEndpointsApiExplorer();
webApplicationBuilder.Services.AddSwaggerGen();

webApplicationBuilder.Services.AddSingleton<WordPieceTokenizer>();

webApplicationBuilder.Services.AddSingleton<OnnxEmbeddingProvider>();

WebApplication webApplication = webApplicationBuilder.Build();

if (webApplication.Environment.IsDevelopment())
{
    webApplication.UseSwagger();
    webApplication.UseSwaggerUI();
}

webApplication.UseAuthorization();
webApplication.MapControllers();
webApplication.Run();