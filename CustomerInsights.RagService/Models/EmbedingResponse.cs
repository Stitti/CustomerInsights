namespace CustomerInsights.RagService.Models;

public class EmbeddingResponse
{
    public string Model { get; set; } =  string.Empty;

    public int Dimension { get; set; }

    public List<EmbeddingVectorModel> Embeddings { get; set; } = new List<EmbeddingVectorModel>();
}