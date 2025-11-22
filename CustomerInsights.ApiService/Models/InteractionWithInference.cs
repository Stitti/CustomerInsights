using CustomerInsights.Models;

namespace CustomerInsights.ApiService.Models
{
    public sealed class InteractionWithInference
    {
        public Interaction Interaction { get; set; } = null!;
        public TextInference? Inference { get; set; }
    }
}
