using CustomerInsights.Analytics;
using CustomerInsights.Base.Models;
using CustomerInsights.InferenceWorker.Repositories;
using CustomerInsights.Models;
using CustomerInsights.Models.Config;

namespace CustomerInsights.Worker
{
    public sealed class InteractionProcessingWorker : BackgroundService
    {
        private readonly ILogger<InteractionProcessingWorker> _logger;
        private readonly OutboxRepository _outboxRepository;
        private readonly InteractionRepository _interactionRepository;
        private readonly SatisfactionStateRepository _satisfactionStateRepository;
        private readonly TextInferenceRepository _textInferenceRepository;

        public InteractionProcessingWorker(OutboxRepository outboxRepository, InteractionRepository interactionRepository, SatisfactionStateRepository satisfactionStateRepository, TextInferenceRepository textInferenceRepository, ILogger<InteractionProcessingWorker> logger)
        {
            _outboxRepository = outboxRepository;
            _interactionRepository = interactionRepository;
            _satisfactionStateRepository = satisfactionStateRepository;
            _textInferenceRepository = textInferenceRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    OutboxMessage? msg = await _outboxRepository.FetchNextOutboxAsync("InteractionCreated", stoppingToken);
                    if (msg == null)
                    {
                        await Task.Delay(500, stoppingToken);
                        continue;
                    }

                    Interaction? interaction = await _interactionRepository.GetInteractionByIdAsync(msg.TargetId, stoppingToken);                        

                    // --- AI-Analyse (Pseudo) ---
                    TextInference inference = await AnalyzeTextAsync(interaction.Text);
                    interaction.TextInference = inference;
                    Guid inferenceId = await _textInferenceRepository.InsertAsync(inference, stoppingToken);
                    await _interactionRepository.MarkAnalyzedAsync(interaction.Id, stoppingToken);

                    // --- Satisfaction berechnen ---
                    AggregationConfig cfg = new AggregationConfig();
                    Weights weights = new();
                    SatisfactionIndexAccumulator acc = await _satisfactionStateRepository.GetAccumulatorAsync(msg.TenantId, interaction.AccountId ?? Guid.Empty, cfg, weights, stoppingToken);

                    acc.AddInteraction(interaction, DateTime.UtcNow);
                    int si = Convert.ToInt32(acc.GetSatisfactionIndex(DateTime.UtcNow));

                    await _satisfactionStateRepository.UpsertAccumulatorAsync(msg.TenantId, interaction.AccountId ?? Guid.Empty, acc, si, stoppingToken);

                    await _outboxRepository.AckOutboxAsync(msg.Id, stoppingToken);

                    _logger.LogInformation("Processed Interaction {Id}, SI={Si:F1}", interaction.Id, si);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in worker loop");
                    await Task.Delay(2000, stoppingToken);
                }
            }
        }

        private static Task<TextInference> AnalyzeTextAsync(string content)
        {
            // Hier z. B. HuggingFace/LLM-API anrufen
            return Task.FromResult(new TextInference
            {
                Sentiment = "POSITIVE",
                SentimentScore = 0.87,
                Urgency = "LOW",
                UrgencyScore = 0.4,
                Emotions = new List<EmotionRating>()
            });
        }
    }
}
