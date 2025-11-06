using System;
using System.Collections.Generic;
using System.Linq;
using CustomerInsights.Models;
using CustomerInsights.Models.Config;

namespace CustomerInsights.Analytics
{
    public sealed class SatisfactionIndexAccumulator
    {
        private const double NaturalLogarithmOfTwo = 0.6931471805599453d;

        private static readonly HashSet<string> NegativeEmotionLabels =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "frustration", "disappointment", "anger", "concern",
                "confusion", "fear", "sadness", "annoyance", "disgust"
            };

        public Guid TenantId { get; }
        public Guid AccountId { get; }

        /// <summary>Σ s_i * w_i nach letztem Decay.</summary>
        public double DecayedWeightedSum { get; private set; }

        /// <summary>Σ w_i nach letztem Decay.</summary>
        public double DecayedWeightSum { get; private set; }

        /// <summary>Zeitpunkt, bis zu dem der Decay angewendet wurde.</summary>
        public DateTime LastUpdatedUtc { get; private set; }

        /// <summary>Konfiguration für Kanalgewichte und Half-Life.</summary>
        public AggregationConfig AggregationConfig { get; }

        /// <summary>Gewichte der Teilkomponenten (Sentiment, Emotion, Urgency).</summary>
        private readonly Weights _weights;

        /// <summary>Optionale Versionierung der Konfiguration (Hash/Versionstring).</summary>
        public string ConfigVersion { get; }

        public SatisfactionIndexAccumulator(
            Guid tenantId,
            Guid accountId,
            AggregationConfig aggregationConfig,
            Weights weights,
            DateTime lastUpdatedUtc,
            double decayedWeightedSum = 0.0d,
            double decayedWeightSum = 0.0d,
            string configVersion = "v1")
        {
            TenantId = tenantId;
            AccountId = accountId;
            AggregationConfig = aggregationConfig ?? new AggregationConfig();
            _weights = weights ?? new Weights();
            LastUpdatedUtc = lastUpdatedUtc;
            DecayedWeightedSum = Math.Max(0.0d, decayedWeightedSum);
            DecayedWeightSum = Math.Max(0.0d, decayedWeightSum);
            ConfigVersion = configVersion ?? "v1";
        }

        /// <summary>
        /// Wendet den zeitlichen Abfall auf beide Summen an, sodass <paramref name="nowUtc"/> der neue Referenzzeitpunkt ist.
        /// </summary>
        public void ApplyTimeDecay(DateTime nowUtc)
        {
            if (nowUtc <= LastUpdatedUtc)
            {
                return;
            }

            double deltaDays = (nowUtc - LastUpdatedUtc).TotalDays;
            double decay = Math.Exp(-NaturalLogarithmOfTwo * (deltaDays / AggregationConfig.HalfLifeDays));

            DecayedWeightedSum *= decay;
            DecayedWeightSum *= decay;
            LastUpdatedUtc = nowUtc;
        }

        /// <summary>
        /// Fügt eine Interaktion identisch zur Batch-Formel hinzu (ohne Tokens/Length-Faktor, da im Original nicht enthalten).
        /// Recency fließt nicht direkt ein; sie ist implizit durch <see cref="ApplyTimeDecay(DateTime)"/> abgebildet.
        /// </summary>
        public void AddInteraction(Interaction interaction, DateTime nowUtc)
        {
            if (interaction == null || interaction.TextInference == null)
            {
                return;
            }

            // 1) Auf nowUtc „nachdecayen“, damit Altersbezug identisch zur Batchformel ist
            ApplyTimeDecay(nowUtc);

            TextInference inference = interaction.TextInference;

            // --- s_i (0..1) exakt wie im Calculator ------------------------------
            double sentimentValue = 0.5d;
            if (String.Equals(inference.Sentiment, "POSITIVE", StringComparison.OrdinalIgnoreCase))
            {
                sentimentValue = 1.0d;
            }
            else if (String.Equals(inference.Sentiment, "NEGATIVE", StringComparison.OrdinalIgnoreCase))
            {
                sentimentValue = 0.0d;
            }

            double urgencyFallback = ClampToUnitInterval(inference.UrgencyScore);
            double urgencyNumeric = 0.5d;
            if (String.Equals(inference.Urgency, "LOW", StringComparison.OrdinalIgnoreCase))
            {
                urgencyNumeric = 0.2d;
            }
            else if (String.Equals(inference.Urgency, "MEDIUM", StringComparison.OrdinalIgnoreCase))
            {
                urgencyNumeric = 0.5d;
            }
            else if (String.Equals(inference.Urgency, "HIGH", StringComparison.OrdinalIgnoreCase))
            {
                urgencyNumeric = 0.8d;
            }
            else
            {
                urgencyNumeric = urgencyFallback;
            }

            IEnumerable<double> negativeEmotionScores = (inference.Emotions ?? Enumerable.Empty<EmotionRating>())
                .Where(e => e != null && e.Label != null && NegativeEmotionLabels.Contains(e.Label))
                .Select(e => ClampToUnitInterval(e.Score));

            double negativeEmotionIntensity = negativeEmotionScores.Any() ? negativeEmotionScores.Average() : 0.0d;

            double emotionComponent = 1.0d - negativeEmotionIntensity;
            double urgencyComponent = 1.0d - urgencyNumeric;

            double satisfactionZeroOne =
                _weights.Sentiment * sentimentValue +
                _weights.Emotion * emotionComponent +
                _weights.Urgency * urgencyComponent;

            satisfactionZeroOne = ClampToUnitInterval(satisfactionZeroOne);

            // --- w_i = ChannelWeight * ConfidenceWeight * UrgencyBoost ----------
            double channelWeight = AggregationConfig.GetWeight(interaction.Channel);

            double sentimentConfidence = ClampToUnitInterval(inference.SentimentScore);
            double confidenceWeight = Math.Max(0.5d, sentimentConfidence);

            double urgencyBoostWeight = 0.5d + 0.5d * urgencyNumeric;

            double staticWeight = channelWeight * confidenceWeight * urgencyBoostWeight;

            // --- Summen aktualisieren -------------------------------------------
            DecayedWeightedSum += satisfactionZeroOne * staticWeight;
            DecayedWeightSum += staticWeight;
        }

        /// <summary>
        /// Liefert den aktuellen Satisfaction Index (0..100) zum Zeitpunkt <paramref name="nowUtc"/>.
        /// </summary>
        public double GetSatisfactionIndex(DateTime nowUtc)
        {
            ApplyTimeDecay(nowUtc);

            if (DecayedWeightSum <= 0.0d)
            {
                return 50.0d;
            }

            double zeroOne = DecayedWeightedSum / DecayedWeightSum;
            double index = Math.Round(ClampToUnitInterval(zeroOne) * 100.0d, 1);
            return index;
        }

        /// <summary>
        /// Rebuild-Helfer: erzeugt einen Accumulator aus einer gegebenen Interaktionsliste (für Backfill/Migration).
        /// Das Ergebnis entspricht der Batch-Berechnung zum Zeitpunkt <paramref name="asOfUtc"/>.
        /// </summary>
        public static SatisfactionIndexAccumulator BuildFromInteractions(
            Guid tenantId,
            Guid accountId,
            IEnumerable<Interaction> interactions,
            AggregationConfig aggregationConfig,
            Weights weights,
            DateTime asOfUtc,
            string configVersion = "v1")
        {
            SatisfactionIndexAccumulator acc = new SatisfactionIndexAccumulator(
                tenantId: tenantId,
                accountId: accountId,
                aggregationConfig: aggregationConfig,
                weights: weights,
                lastUpdatedUtc: asOfUtc,
                decayedWeightedSum: 0.0d,
                decayedWeightSum: 0.0d,
                configVersion: configVersion);

            // Wir fügen alle Interaktionen in beliebiger Reihenfolge zu,
            // wenden aber vor der finalen Abfrage GetSatisfactionIndex(asOfUtc) keinen weiteren Decay mehr an,
            // da AddInteraction(...) immer auf asOfUtc decayed.
            foreach (Interaction interaction in interactions ?? Enumerable.Empty<Interaction>())
            {
                if (interaction == null || interaction.TextInference == null)
                {
                    continue;
                }

                // Bei Batch-Rebuild gilt: der Recency-Faktor der Batchformel
                // ist äquivalent zum Decay (asOfUtc - OccurredAt). Dies wird erreicht,
                // indem wir zuerst auf OccurredAt decayed und dann die Interaktion ohne Recency direkt addieren.
                // Praktisch: wir decayern auf Interaktionszeitpunkt, addieren, und decayern wieder auf asOfUtc.
                DateTime occurred = interaction.OccurredAt;
                if (occurred < acc.LastUpdatedUtc)
                {
                    // Falls historische Reihenfolge rückwärts ist, zuerst auf occurred decayern.
                    acc.ApplyTimeDecay(occurred);
                    acc.AddInteraction(interaction, occurred);
                    acc.ApplyTimeDecay(asOfUtc);
                }
                else
                {
                    // Normale zeitliche Reihenfolge
                    acc.ApplyTimeDecay(occurred);
                    acc.AddInteraction(interaction, occurred);
                    acc.ApplyTimeDecay(asOfUtc);
                }
            }

            return acc;
        }

        private static double ClampToUnitInterval(double value)
        {
            if (value < 0.0d) return 0.0d;
            if (value > 1.0d) return 1.0d;
            return value;
        }
    }
}