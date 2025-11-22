namespace CustomerInsights.ApiService.Models
{
    public sealed class SatisfactionStateData
    {
        public double DecayedWeightedSum { get; set; }
        public double DecayedWeightSum { get; set; }
        public double SatisfactionIndex { get; set; }
    }
}
