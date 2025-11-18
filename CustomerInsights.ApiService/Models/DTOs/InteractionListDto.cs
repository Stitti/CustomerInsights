using CustomerInsights.Base.Enums;

namespace CustomerInsights.ApiService.Models.DTOs
{
    public class InteractionListDto
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

        public TextInferenceListDto? TextInference { get; set; }
    }
}
