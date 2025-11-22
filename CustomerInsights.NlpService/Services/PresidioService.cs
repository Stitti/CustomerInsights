using CustomerInsights.NlpService.Contracts;
using Newtonsoft.Json;
using System.Text;

namespace CustomerInsights.NlpService.Services;

public sealed class PresidioService
{
    private readonly HttpClient _analyzerHttpClient;
    private readonly HttpClient _anonymizerHttpClient;
    private readonly ILogger<PresidioService> _logger;
    private static readonly string[] _entities = new string[] { "PERSON", "EMAIL_ADDRESS", "PHONE_NUMBER", "ORGANIZATION" };
    private static readonly object _anonymizers = new
    {
        DEFAULT = new PresidioAnonymizeEntity { Type = "replace", AnonymizedValue = "<PII>" },
        PERSON = new PresidioAnonymizeEntity { Type = "replace", AnonymizedValue = "<NAME>" },
        EMAIL_ADDRESS = new PresidioAnonymizeEntity { Type = "replace", AnonymizedValue = "<EMAIL>" },
        PHONE_NUMBER = new PresidioAnonymizeEntity { Type = "replace", AnonymizedValue = "<PHONE>" },
        ORGANIZATION = new PresidioAnonymizeEntity { Type = "replace", AnonymizedValue = "<ORG>" }
    };

    public PresidioService(HttpClient analyzerHttpClient, HttpClient anonymizerHttpClient, ILogger<PresidioService> logger)
    {
        _analyzerHttpClient = analyzerHttpClient ?? throw new ArgumentNullException(nameof(analyzerHttpClient));
        _anonymizerHttpClient = anonymizerHttpClient ?? throw new ArgumentNullException(nameof(anonymizerHttpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PresidioAnalyzeAndAnonymizeResult> AnalyzeAndAnonymizeAsync(string text, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            PresidioAnalyzeAndAnonymizeResult emptyResult = new PresidioAnalyzeAndAnonymizeResult
            {
                AnonymizedText = text,
                Entities = new List<PresidioAnalyzerEntity>()
            };

            return emptyResult;
        }

        List<PresidioAnalyzerEntity> entities = await AnalyzeAsync(text, cancellationToken);
        string anonymizedText = await AnonymizeAsync(text, entities, cancellationToken);

        PresidioAnalyzeAndAnonymizeResult result = new PresidioAnalyzeAndAnonymizeResult
        {
            AnonymizedText = anonymizedText,
            Entities = entities
        };

        return result;
    }

    private async Task<List<PresidioAnalyzerEntity>> AnalyzeAsync(string text, CancellationToken cancellationToken)
    {
        PresidioAnalyzeRequest requestObject = new PresidioAnalyzeRequest
        {
            Text = text,
            Language = "de",
            Entities = _entities
        };

        string requestJson = JsonConvert.SerializeObject(requestObject);

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "/analyze")
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };

        HttpResponseMessage responseMessage = await _analyzerHttpClient.SendAsync(requestMessage, cancellationToken);
        responseMessage.EnsureSuccessStatusCode();

        string responseJson = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        List<PresidioAnalyzerEntity>? entities = JsonConvert.DeserializeObject<List<PresidioAnalyzerEntity>>(responseJson);

        if (entities == null)
        {
            return new List<PresidioAnalyzerEntity>();
        }

        return entities;
    }

    private async Task<string> AnonymizeAsync(string text, List<PresidioAnalyzerEntity> entities, CancellationToken cancellationToken)
    {
        PresidioAnonymizeRequest requestObject = new PresidioAnonymizeRequest
        {
            Text = text,
            AnalyzerResults = entities,
            Anonymizers = _anonymizers
        };

        string requestJson = JsonConvert.SerializeObject(requestObject);

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "/anonymize")
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
        };

        HttpResponseMessage responseMessage = await _anonymizerHttpClient.SendAsync(requestMessage, cancellationToken);
        responseMessage.EnsureSuccessStatusCode();

        string responseJson = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        PresidioAnonymizeResponse? anonymizeResponse = JsonConvert.DeserializeObject<PresidioAnonymizeResponse>(responseJson);

        if (anonymizeResponse == null)
        {
            return text;
        }

        return anonymizeResponse.Text;
    }
}
