namespace CustomerInsights.EmbeddingService.Contracts;

public class EmbedVector
{
    public int Index { get; set; }
    public double[] Vector { get; set; } = Array.Empty<double>();
}