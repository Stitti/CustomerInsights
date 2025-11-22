using CustomerInsights.Base.Models;
using CustomerInsights.Models;
using CustomerInsights.SatisfactionIndexService.Models;
using CustomerInsights.SatisfactionIndexService.Repositories;

public sealed class SatisfactionIndexRecalculationService
{
    private readonly SatisfactionStateRepository _repository;
    private readonly Weights _weights;
    private readonly AggregationConfig _aggregationConfig;
    private readonly ILogger<SatisfactionIndexRecalculationService> _logger;

    private static readonly HashSet<string> NegativeEmotionLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "frustration", "disappointment", "anger", "concern", "confusion", "fear", "sadness", "annoyance", "disgust" };

    private const double NaturalLogarithmOfTwo = 0.6931471805599453d;

    public SatisfactionIndexRecalculationService(SatisfactionStateRepository repository, Weights weights, AggregationConfig aggregationConfig, ILogger<SatisfactionIndexRecalculationService> logger)
    {
        _repository = repository;
        _weights = weights;
        _aggregationConfig = aggregationConfig;
        _logger = logger;
    }

    public async Task<SatisfactionRecalculationResult?> UpdateForAnalyzedInteractionAsync(Guid tenantId, Interaction interaction, CancellationToken cancellationToken)
    {
        if (interaction == null)
        {
            _logger.LogError("Interaction is invalid for tenant {TenantId}", tenantId);
            return null;
        }

        if (interaction.AccountId == null || interaction.AccountId == Guid.Empty)
        {
            _logger.LogWarning("Interaction must have an AccountId to update satisfaction index for tenant {TenantId}", tenantId);
            return null;
        }

        if (interaction.TextInference == null)
        {
            _logger.LogError("Interaction must have a TextInference to update satisfaction index for tenant {TenantId}", tenantId);
            return null;
        }

        DateTime nowUtc = DateTime.UtcNow;

        SatisfactionState? state = await _repository.GetByAccountIdAsync(tenantId, interaction.AccountId.Value, cancellationToken);

        if (state == null)
        {
            state = new SatisfactionState
            {
                TenantId = tenantId,
                AccountId = interaction.AccountId.Value,
                LastUpdatedUtc = nowUtc,
                SatisfactionIndex = 50,
                DecayedWeightedSum = 0.0d,
                DecayedWeightSum = 0.0d,
                ConfigVersion = "v1"
            };

            await _repository.AddAsync(state, cancellationToken);
        }

        // Bisherigen Zustand nach Zeitunterschied "verfallen" lassen
        double deltaDays = Math.Max(0.0d, (nowUtc - state.LastUpdatedUtc).TotalDays);
        double decayFactor = 1.0d;

        if (_aggregationConfig.HalfLifeDays > 0.0d && deltaDays > 0.0d)
        {
            decayFactor = Math.Exp(-NaturalLogarithmOfTwo * deltaDays / _aggregationConfig.HalfLifeDays);
        }

        double decayedWeightedSum = state.DecayedWeightedSum * decayFactor;
        double decayedWeightSum = state.DecayedWeightSum * decayFactor;

        TextInference inference = interaction.TextInference;

        double sentimentValue = 0.5d;
        if (string.Equals(inference.Sentiment, "POSITIVE", StringComparison.OrdinalIgnoreCase))
        {
            sentimentValue = 1.0d;
        }
        else if (string.Equals(inference.Sentiment, "NEGATIVE", StringComparison.OrdinalIgnoreCase))
        {
            sentimentValue = 0.0d;
        }

        double sentimentConfidence = ClampToUnitInterval(inference.SentimentScore);

        double urgencyNumeric = 0.5d;
        double urgencyFallback = ClampToUnitInterval(inference.UrgencyScore);
        if (string.Equals(inference.Urgency, "LOW", StringComparison.OrdinalIgnoreCase))
        {
            urgencyNumeric = 0.2d;
        }
        else if (string.Equals(inference.Urgency, "MEDIUM", StringComparison.OrdinalIgnoreCase))
        {
            urgencyNumeric = 0.5d;
        }
        else if (string.Equals(inference.Urgency, "HIGH", StringComparison.OrdinalIgnoreCase))
        {
            urgencyNumeric = 0.8d;
        }
        else
        {
            urgencyNumeric = urgencyFallback;
        }

        IEnumerable<double> negativeEmotionScores = (inference.Emotions ?? new List<EmotionRating>())
            .Where(e => e != null && e.Label != null && NegativeEmotionLabels.Contains(e.Label))
            .Select(e => ClampToUnitInterval(e.Score));

        double negativeEmotionIntensity = negativeEmotionScores.Any()
            ? negativeEmotionScores.Average()
            : 0.0d;

        double emotionComponent = 1.0d - negativeEmotionIntensity;
        double urgencyComponent = 1.0d - urgencyNumeric;

        double satisfactionZeroOne = _weights.Sentiment * sentimentValue +
                                     _weights.Emotion * emotionComponent +
                                     _weights.Urgency * urgencyComponent;

        satisfactionZeroOne = ClampToUnitInterval(satisfactionZeroOne);

        double channelWeight = _aggregationConfig.GetWeight(interaction.Channel);
        double confidenceWeight = Math.Max(0.5d, sentimentConfidence);
        double urgencyWeight = 0.5d + 0.5d * urgencyNumeric;
        double interactionWeight = channelWeight * confidenceWeight * urgencyWeight;

        double newWeightedSum = decayedWeightedSum + satisfactionZeroOne * interactionWeight;
        double newWeightSum = decayedWeightSum + interactionWeight;

        double accountSatisfactionIndexZeroOne = newWeightSum > 0.0d
            ? newWeightedSum / newWeightSum
            : 0.5d;

        int accountSatisfactionIndex = (int)(ClampToUnitInterval(accountSatisfactionIndexZeroOne) * 100.0d);

        state.LastUpdatedUtc = nowUtc;
        state.DecayedWeightedSum = newWeightedSum;
        state.DecayedWeightSum = newWeightSum;
        state.SatisfactionIndex = accountSatisfactionIndex;

        if (string.IsNullOrWhiteSpace(state.ConfigVersion))
        {
            state.ConfigVersion = "v1";
        }

        await _repository.SaveChangesAsync(cancellationToken);

        List<SatisfactionState> tenantStates = await _repository.GetAllByTenantIdAsync(tenantId, cancellationToken);

        double sumNumerator = tenantStates.Sum(s => s.DecayedWeightedSum);
        double sumDenominator = tenantStates.Sum(s => s.DecayedWeightSum);

        double tenantFallback = tenantStates.Count > 0
            ? tenantStates.Average(s => s.SatisfactionIndex)
            : accountSatisfactionIndex;

        double tenantSatisfactionIndex = sumDenominator > 0.0d
            ? Math.Round(ClampToUnitInterval(sumNumerator / sumDenominator) * 100.0d, 1)
            : Math.Round(tenantFallback, 1);

        SatisfactionRecalculationResult result = new SatisfactionRecalculationResult
        {
            AccountSatisfactionIndex = accountSatisfactionIndex,
            TenantSatisfactionIndex = tenantSatisfactionIndex
        };

        return result;
    }

    private static double ClampToUnitInterval(double value)
    {
        if (value < 0.0d)
        {
            return 0.0d;
        }

        if (value > 1.0d)
        {
            return 1.0d;
        }

        return value;
    }
}