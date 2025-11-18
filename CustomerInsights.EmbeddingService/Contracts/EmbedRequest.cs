namespace CustomerInsights.EmbeddingService.Contracts;

public class EmbedRequest
{
    public List<string> Texts { get; set; } = new List<string>();
    public string? Model { get; set; }
}