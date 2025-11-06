using CustomerInsights.Base.Models.Responses;
using System.Text.Json;

namespace CustomerInsights.NlpService.Runtime;

public sealed class TextAnalyzer
{
    private readonly SentimentOnnx3 _sentimentModel;
    private readonly ZeroShotAspectNliOnnx _aspectNliModel;
    private readonly EmotionOnnxMulti _emotionModel;
    private readonly UrgencyOnnx3 _urgencyModel;

    public TextAnalyzer(SentimentOnnx3 sentimentModel, ZeroShotAspectNliOnnx aspectNliModel, EmotionOnnxMulti emotionModel, UrgencyOnnx3 urgencyModel)
    {
        if (sentimentModel == null)
            throw new ArgumentNullException(nameof(sentimentModel));

        if (aspectNliModel == null)
            throw new ArgumentNullException(nameof(aspectNliModel));

        if (emotionModel == null)
            throw new ArgumentNullException(nameof(emotionModel));

        if (urgencyModel == null)
            throw new ArgumentNullException(nameof(urgencyModel));

        _sentimentModel = sentimentModel;
        _aspectNliModel = aspectNliModel;
        _emotionModel = emotionModel;
        _urgencyModel = urgencyModel;
    }

    public NlpResponse Analyze(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text must not be null or whitespace.", nameof(text));
        }

        text = text.Trim();

        ScoreResult sentiment = _sentimentModel.Predict(text);

        string[] aspects = LoadAspects();
        ScoreResult[] aspectScores = _aspectNliModel.ScoreAspects(text, aspects);

        ScoreResult[] emotions = _emotionModel.Score(text);

        ScoreResult urgency = _urgencyModel.Predict(text);

        return new NlpResponse
        {
            Aspects = aspectScores,
            Emotions = emotions,
            Sentiment = sentiment,
            Urgency = urgency
        };
    }

    private string[] LoadAspects()
    {
        string aspectsPath = Path.Combine(AppContext.BaseDirectory, "Config", "aspects.json");
        if (File.Exists(aspectsPath) == false)
        {
            throw new FileNotFoundException("Aspektliste wurde nicht gefunden.", aspectsPath);
        }

        string aspectsJson = File.ReadAllText(aspectsPath);
        string[] parsedAspects = JsonSerializer.Deserialize<string[]>(aspectsJson) ?? throw new InvalidOperationException("Aspektliste konnte nicht gelesen werden.");
        return parsedAspects.Where(static s => string.IsNullOrWhiteSpace(s) == false)
                            .Select(static s => s.Trim())
                            .ToArray();
    }
}
