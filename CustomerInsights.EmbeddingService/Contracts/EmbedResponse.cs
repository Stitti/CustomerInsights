namespace CustomerInsights.EmbeddingService.Contracts;

public class EmbedResponse
{
    public string Model { get; set; } = string.Empty;
    public int Dimension { get; set; }
    public List<EmbedVector> Embeddings { get; set; } = new List<EmbedVector>();
}