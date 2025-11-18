using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerInsights.RagService.Models;

namespace CustomerInsights.RagService.Services
{
    public class RagQueryService
    {
        private readonly EmbeddingClient _embeddingClient;
        private readonly InteractionRepository _interactionRepository;
        private readonly OllamaChatClient _chatClient;

        public RagQueryService(EmbeddingClient embeddingClient, InteractionRepository interactionRepository, OllamaChatClient chatClient)
        {
            _embeddingClient = embeddingClient;
            _interactionRepository = interactionRepository;
            _chatClient = chatClient;
        }

        public async Task<RagResponse> QueryAsync(RagRequest request)
        {
            double[] queryEmbedding = await _embeddingClient.CreateEmbeddingAsync(request.Question);

            IList<RagDocument> documents = await _interactionRepository.GetRelevantDocumentsAsync(
                queryEmbedding,
                request.CompanyId,
                request.Product,
                request.Sentiment,
                request.From,
                request.To,
                0);

            string answer;

            if (documents.Count == 0)
            {
                answer = "Ich konnte keine passenden Interaktionen zur Frage finden.";
            }
            else
            {
                answer = await _chatClient.GenerateAnswerAsync(request.Question, documents);
            }

            RagResponse response = new RagResponse();
            response.Answer = answer;
            response.Documents = new List<RagDocument>(documents);

            return response;
        }
    }
}