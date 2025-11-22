using CustomerInsights.Base.Enums;
using CustomerInsights.Models;
using System.Text.Json;

namespace CustomerInsights.ApiService.Models.DTOs
{
    public sealed class InteractionDto
    {
        public Guid Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
        public Channel Channel { get; set; }

        public DateTime OccurredAt { get; set; }
        public DateTime AnalyzedAt { get; set; }

        public AccountListDto? Account { get; set; }
        public ContactListDto? Contact { get; set; }

        public string Subject { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        public TextInferenceDto? TextInference { get; set; }
    }
}
