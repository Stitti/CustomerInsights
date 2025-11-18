using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CustomerInsights.RagService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CustomerInsights.RagService.Services
{
    public class OllamaChatClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _modelName;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<OllamaChatClient> _logger;

        public OllamaChatClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<OllamaChatClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            string? modelFromConfig = configuration["ChatService:Model"];
            if (string.IsNullOrWhiteSpace(modelFromConfig))
            {
                throw new InvalidOperationException("ChatService:Model is not configured.");
            }

            _modelName = modelFromConfig;

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            _jsonSerializerOptions = options;
        }

        public async Task<string> GenerateAnswerAsync(string question, IList<RagDocument> documents)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("ChatClient");

            StringBuilder contextBuilder = new StringBuilder();
            contextBuilder.AppendLine("Du bist ein Analyse-Assistent für Kundeninteraktionen.");
            contextBuilder.AppendLine("Beantworte die Frage ausschließlich auf Basis der folgenden Interaktionen.");
            contextBuilder.AppendLine("Wenn Informationen fehlen, sag das klar.");
            contextBuilder.AppendLine();
            contextBuilder.AppendLine("Frage:");
            contextBuilder.AppendLine(question);
            contextBuilder.AppendLine();
            contextBuilder.AppendLine("Interaktionen:");

            foreach (RagDocument document in documents)
            {
                contextBuilder.AppendLine("-----");
                contextBuilder.AppendLine("InteraktionId: " + document.InteractionId);
                contextBuilder.AppendLine("FirmaId: " + (document.CompanyId.HasValue ? document.CompanyId.Value.ToString() : "null"));
                contextBuilder.AppendLine("KontaktId: " + (document.ContactId.HasValue ? document.ContactId.Value.ToString() : "null"));
                contextBuilder.AppendLine("Kanal: " + document.Channel);
                contextBuilder.AppendLine("Emotion: " + document.Emotion);
                contextBuilder.AppendLine("Produkte: " + (document.Products != null ? string.Join(", ", document.Products) : string.Empty));
                contextBuilder.AppendLine("Tags: " + (document.Tags != null ? string.Join(", ", document.Tags) : string.Empty));
                contextBuilder.AppendLine("Datum: " + document.CreatedAt.ToString("O"));
                contextBuilder.AppendLine();
                contextBuilder.AppendLine(document.TextFull);
                contextBuilder.AppendLine();
            }

            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload["model"] = _modelName;
            payload["prompt"] = contextBuilder.ToString();
            payload["stream"] = false;

            string requestJson = JsonSerializer.Serialize(payload, _jsonSerializerOptions);
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync("/api/generate", content);

            if (response.IsSuccessStatusCode == false)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Chat request failed: {Error}", errorBody);
                throw new InvalidOperationException("Chat request failed.");
            }

            string responseJson = await response.Content.ReadAsStringAsync();
            JsonDocument jsonDocument = JsonDocument.Parse(responseJson);

            if (jsonDocument.RootElement.TryGetProperty("response", out JsonElement responseElement) == false)
            {
                throw new InvalidOperationException("Chat response missing 'response' field.");
            }

            return responseElement.GetString() ?? string.Empty;
        }
    }
}