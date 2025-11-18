namespace CustomerInsights.EmbeddingService.Contracts;

public class WordPieceTokenizerAdapterOptions
{
    public string VocabFilePath { get; set; } = string.Empty;
    public bool LowerCase { get; set; } = true;
    public string PadToken { get; set; } = "[PAD]";
}