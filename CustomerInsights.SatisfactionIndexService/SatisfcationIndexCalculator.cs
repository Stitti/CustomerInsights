using CustomerInsights.Models;
using CustomerInsights.SatisfactionIndexService.Models;

namespace CustomerInsights.SatisfactionIndexService
{
    public static class SatisfactionIndexCalculator
    {
        private static readonly HashSet<string> NegativeEmotionLabels = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "frustration", "disappointment", "anger", "concern", "confusion", "fear", "sadness", "annoyance", "disgust" };
        private const double Ln2 = 0.6931471805599453d;

        public static double ComputeForAccount(IEnumerable<Interaction> interactions, Weights weights = null, AggregationConfig config = null, DateTime? nowUtc = null)
        {
            if (interactions == null)
                return 50.0d;

            weights ??= new Weights();
            config ??= new AggregationConfig();
            nowUtc ??= DateTime.UtcNow;

            List<WeightedInteraction> items = new List<WeightedInteraction>();

            foreach (Interaction interaction in interactions)
            {
                if (interaction == null || interaction.TextInference == null)
                    continue;

                TextInference textInference = interaction.TextInference;

                // --- Sentiment ---
                double sentimentConfidence = Clamp01(textInference.SentimentScore);
                double sentimentValue = textInference.Sentiment?.ToUpperInvariant() switch
                {
                    "POSITIVE" => 1.0d,
                    "NEGATIVE" => 0.0d,
                    _ => 0.5d
                };

                // --- Urgency ---
                double urgencyNumerical = textInference.Urgency?.ToUpperInvariant() switch
                {
                    "LOW" => 0.2d,
                    "MEDIUM" => 0.5d,
                    "HIGH" => 0.8d,
                    _ => Clamp01(textInference.UrgencyScore)
                };

                // --- Emotions (negativ) ---
                IEnumerable<double> negativeScores = textInference.Emotions.Where(e => NegativeEmotionLabels.Contains(e.Label))
                                                                           .Select(e => Clamp01(e.Score));

                double negIntensity = negativeScores.Count() > 0 ? negativeScores.Average() : 0d;

                double emotionComponent = 1.0d - negIntensity;
                double urgencyComponent = 1.0d - urgencyNumerical;

                double si01 = Clamp01(weights.Sentiment * sentimentValue + weights.Emotion * emotionComponent + weights.Urgency * urgencyComponent);

                // --- Recency Weight ---
                double ageDays = Math.Max(0.0d, (nowUtc - interaction.OccurredAt).Value.TotalDays);
                double recency = Math.Exp(-Ln2 * ageDays / config.HalfLifeDays);

                // --- Other weights ---
                double channelWeight = config.GetWeight(interaction.Channel);
                double confidenceWeight = Math.Max(0.5d, sentimentConfidence);
                double urgencyWeight = 0.5d + 0.5d * urgencyNumerical;

                double totalWeight = recency * channelWeight * confidenceWeight * urgencyWeight;

                items.Add(new WeightedInteraction
                {
                    SatisfactionZeroOne = si01,
                    Weight = totalWeight
                });
            }

            if (items.Count == 0)
            {
                return 50.0d;
            }

            double numerator = items.Sum(i => i.SatisfactionZeroOne * i.Weight);
            double denominator = items.Sum(i => i.Weight);
            double siZeroOne = denominator > 0 ? numerator / denominator : 0.5d;
            return Math.Round(Clamp01(siZeroOne) * 100.0d, 1);
        }

        private static double Clamp01(double value)
        {
            if (value < 0d) 
                return 0d;

            if (value > 1d) 
                return 1d;

            return value;
        }
    }
}
