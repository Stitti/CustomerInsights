namespace CustomerInsights.EmbeddingService.Contracts;

public class TokenizationResult
{
    public int[] InputIds { get; set; } = Array.Empty<int>();
    public int[] AttentionMask { get; set; } = Array.Empty<int>();
}