using CustomerInsights.Base.Models.Responses;
using CustomerInsights.Models;
using CustomerInsights.NlpService.Repositories;

namespace CustomerInsights.NlpService.Services
{
    public sealed class TextInferenceService
    {
        private readonly TextInferenceRepository _repository;
        private readonly ILogger<TextInferenceService> _logger;

        public TextInferenceService(TextInferenceRepository repository, ILogger<TextInferenceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> CreateTextInferenceAsync(Guid interactionId, Guid tenantId, NlpResponse nlpResponse, CancellationToken cancellationToken = default)
        {
            TextInference textInference = new TextInference
            {
                InteractionId = interactionId,
                Sentiment = nlpResponse.Sentiment.Label,
                SentimentScore = nlpResponse.Sentiment.Score,
                Urgency = nlpResponse.Urgency.Label,
                UrgencyScore = nlpResponse.Urgency.Score,
                Aspects = nlpResponse.Aspects.Select(x => new AspectRating { AspectName = x.Label, Score = x.Score }).ToList(),
                Emotions = nlpResponse.Emotions.Select(x => new EmotionRating { Label = x.Label, Score = x.Score }).ToList(),
                InferredAt = DateTime.UtcNow
            };

            return await _repository.CreateTextInferenceAsync(tenantId, textInference, cancellationToken);
        }
    }
}
