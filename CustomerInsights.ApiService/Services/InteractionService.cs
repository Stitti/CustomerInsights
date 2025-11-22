using CustomerInsights.ApiService.Database.Repositories;
using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.ApiService.Utils;
using CustomerInsights.Models;
using CustomerInsights.NlpService.Contracts;
using Newtonsoft.Json;


namespace CustomerInsights.ApiService.Services
{
    public sealed class InteractionService
    {
        private readonly InteractionRepository _repository;
        private readonly TextNormalizer _textNormalizer;
        private readonly RabbitMqSenderService _rabbitMqSenderService;
        private readonly ILogger<InteractionService> _logger;

        public InteractionService(InteractionRepository repository, TextNormalizer textNormalizer, RabbitMqSenderService rabbitMqSenderService, ILogger<InteractionService> logger)
        {
            _repository = repository;
            _textNormalizer = textNormalizer;
            _rabbitMqSenderService = rabbitMqSenderService;
            _logger = logger;
        }

        public async Task<InteractionDto?> GetInteractionByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.GetInteractionByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<InteractionListDto>> GetAllInteractionsAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllInteractionsAsync(cancellationToken);
        }

        public async Task IngestAsync(Guid tenantId, IngestInteractionRequest request, CancellationToken cancellationToken = default)
        {
            Interaction interaction = MapToInteraction(tenantId, request);
            interaction.Text = _textNormalizer.Normalize(interaction.Text, interaction.Channel);

            Guid interactionId = await _repository.InsertAsync(interaction, cancellationToken);

            NlpJobMessage message = new NlpJobMessage
            {
                Id = interactionId,
                TenantId = tenantId,
                Message = interaction.Text
            };

            string json = JsonConvert.SerializeObject(message);
            await _rabbitMqSenderService.SendMessageAsync("nlp_jobs", json, cancellationToken);
            _logger.LogInformation("Interaction {InteractionId} queued for analysis", interactionId);
        }

        internal async Task<IReadOnlyList<ChannelCount>> GetTopChannelsAsync(TimeInterval period, CancellationToken cancellationToken = default)
        {
            return await _repository.GetTopChannelsAsync(period, cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _repository.DeleteAsync(id, cancellationToken);
        }

        internal async Task<bool> PatchAsync(Guid id, UpdateInteractionRequest request, CancellationToken cancellationToken = default)
        {
            return await _repository.PatchAsync(id, request, cancellationToken);
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
    }
}
