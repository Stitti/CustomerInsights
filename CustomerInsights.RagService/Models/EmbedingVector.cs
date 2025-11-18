namespace CustomerInsights.RagService.Models;

public class EmbeddingVectorModel
{
    public int Index { get; set; }
    public double[] Vector { get; set; } = Array.Empty<double>();

}