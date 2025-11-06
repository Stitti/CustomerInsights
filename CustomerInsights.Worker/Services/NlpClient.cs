using CustomerInsights.Base.Models.Requests;
using CustomerInsights.Base.Models.Responses;
using System.Net.Http.Json;

namespace CustomerInsights.Worker.Services;

public sealed class NlpClient
{
    private readonly HttpClient _http;
    public NlpClient()
    {
        _http = new HttpClient();
    }

    public async Task<NlpResponse> AnalyzeAsync(string text, string? hintLang = null, string? channel = null, CancellationToken ct = default)
    {
        NlpRequest req = new NlpRequest { Text = text, HintLanguage = hintLang, Channel = channel };
        using HttpResponseMessage res = await _http.PostAsJsonAsync("/analyze", req, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<NlpResponse>(cancellationToken: ct))!;
    }
}
