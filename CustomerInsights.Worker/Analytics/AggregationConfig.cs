using System;
using System.Collections.Generic;
using CustomerInsights.Base.Enums;
using CustomerInsights.Models;

namespace CustomerInsights.Analytics
{
    public sealed class AggregationConfig
    {
        /// <summary>
        /// Halbwertszeit in Tagen, nach der das Gewicht eines Feedbacks
        /// zeitlich auf 50 % seiner ursprünglichen Stärke abnimmt.
        /// </summary>
        public double HalfLifeDays { get; init; } = 30.0d;

        public IReadOnlyDictionary<Channel, double> ChannelWeights { get; }

        public AggregationConfig()
        {
            ChannelWeights = new Dictionary<Channel, double>
            {
                // Direkte Kommunikationskanäle
                { Channel.Email,       0.9d },   // eher asynchron, weniger Kontext
                { Channel.Ticket,      1.1d },   // Support Tickets → oft kritische Feedbacks
                { Channel.Call,        1.3d },   // Telefonate → hoher emotionaler Gehalt
                { Channel.Meeting,     1.2d },   // persönliche Treffen → qualitativ wertvoll
                { Channel.Chat,        1.0d },   // schnelle Kommunikation, mittlere Relevanz

                // Indirekte / öffentliche Feedback-Kanäle
                { Channel.Survey,      1.0d },   // strukturierte Zufriedenheitsbefragung
                { Channel.Review,      0.8d },   // öffentliche Rezensionen → tendenziell extremer
                { Channel.Social,      0.7d },   // Social Media → hoher Lärmanteil

                // Interne / vertriebsorientierte Quellen
                { Channel.VisitReport, 1.1d },   // Kundenbesuche → direktes Feedback
                { Channel.CrmNote,     0.9d },   // interne Notizen → subjektiver
                { Channel.NpsSurvey,   1.0d },   // standardisierte Kennzahl → baseline

                // Sonstige / unbekannte Kanäle
                { Channel.Unknown,     1.0d },
                { Channel.Other,       1.0d }
            };
        }

        public double GetWeight(Channel channel)
        {
            if (ChannelWeights.TryGetValue(channel, out double weight))
            {
                return weight;
            }

            return 1.0d;
        }
    }
}
