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
    public class EmbeddingClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<EmbeddingClient> _logger;

        public EmbeddingClient(
            IHttpClientFactory httpClientFactory,
            ILogger<EmbeddingClient> logger)
        {
            _httpClientFactory = httpClientFactory;

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            _jsonSerializerOptions = options;

            _logger = logger;
        }

        public async Task<double[]> CreateEmbeddingAsync(string text)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("EmbeddingClient");

            Dictionary<string, object> payload = new Dictionary<string, object>();
            payload["texts"] = new List<string> { text };
            payload["model"] = null;

            string requestJson = JsonSerializer.Serialize(payload, _jsonSerializerOptions);
            StringContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync("/embedding/embed", content);

            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Embedding request failed: {Error}", errorBody);
                throw new InvalidOperationException("Embedding request failed.");
            }

            string responseJson = await response.Content.ReadAsStringAsync();
            EmbeddingResponse? embeddingResponse =
                JsonSerializer.Deserialize<EmbeddingResponse>(responseJson, _jsonSerializerOptions);

            if (embeddingResponse == null ||
                embeddingResponse.Embeddings == null ||
                embeddingResponse.Embeddings.Count == 0)
            {
                throw new InvalidOperationException("Embedding response is empty.");
            }

            return embeddingResponse.Embeddings[0].Vector;
        }
    }
}