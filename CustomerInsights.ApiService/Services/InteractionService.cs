using CustomerInsights.ApiService.Database.Repositories;
using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Utils;
using CustomerInsights.Models;


namespace CustomerInsights.ApiService.Services
{
    public sealed class InteractionService
    {
        private readonly InteractionRepository _repository;
        private readonly TextNormalizer _textNormalizer;
        private readonly ILogger<InteractionService> _logger;

        public InteractionService(InteractionRepository repository, TextNormalizer textNormalizer, ILogger<InteractionService> logger)
        {
            _repository = repository;
            _textNormalizer = textNormalizer;
            _logger = logger;
        }

        public async Task<Interaction?> GetInteractionByIdAsync(Guid id)
        {
            return await _repository.GetInteractionByIdAsync(id);
        }

        public async Task<IEnumerable<Interaction>> GetAllInteractionsAsync()
        {
            return await _repository.GetAllInteractionsAsync();
        }

        public async Task IngestAsync(Guid tenantId, IngestInteractionRequest request)
        {
            Interaction interaction = MapToInteraction(tenantId, request);
            interaction.Text = _textNormalizer.Normalize(interaction.Text, interaction.Channel);

            Guid interactionId = await _repository.InsertAsync(interaction);


            _logger.LogInformation("Interaction {InteractionId} queued for analysis", interactionId);
        }

        public async Task<BatchIngestResponse> IngestBatchAsync(Guid tenantId, IngestInteractionRequest[] requests)
        {
            var response = new BatchIngestResponse
            {
                TotalSubmitted = requests.Length
            };

            var interactions = new List<Interaction>(requests.Length);
            var errors = new List<string>();

            // 1. Map and normalize all interactions
            for (int i = 0; i < requests.Length; i++)
            {
                try
                {
                    Interaction interaction = MapToInteraction(tenantId, requests[i]);
                    interaction.Text = _textNormalizer.Normalize(interaction.Text, interaction.Channel);

                    interactions.Add(interaction);
                }
                catch (Exception ex)
                {
                    errors.Add($"Item {i}: {ex.Message}");
                    _logger.LogWarning(ex, "Failed to map interaction at index {Index}", i);
                }
            }

            // 2. Bulk insert
            if (interactions.Count > 0)
            {
                Guid[] ids = await _repository.InsertBatchAsync(interactions);

                // 3. Enqueue all for analysis
                // TODO               

                response.SuccessfullyQueued = ids.Length;
                response.InteractionIds = ids;
            }

            response.Failed = errors.Count;
            response.Errors = errors.Count > 0 ? errors : null;

            return response;
        }

        private static Interaction MapToInteraction(Guid tenantId, IngestInteractionRequest request)
        {
            return new Interaction
            {
                TenantId = tenantId,
                AccountId = request.AccountId,
                ContactId = request.ContactId,
                ThreadId = request.ThreadId,
                Channel = request.Channel,
                ExternalId = request.ExternalId,
            };
        }

        internal async Task<IReadOnlyList<ChannelCount>> GetTopChannelsAsync(Period period, DateTimeOffset utcNow, DateTimeOffset? fromUtc, DateTimeOffset? toUtc)
        {
            return await _repository.GetTopChannelsAsync(period, utcNow, fromUtc, toUtc);
        }
    }
}
