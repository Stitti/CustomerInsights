using CustomerInsights.EmbeddingService.Contracts;
using CustomerInsights.EmbeddingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerInsights.EmbeddingService.Controllers;

[ApiController]
[Route("[controller]")]
public class EmbeddingController : ControllerBase
{
    private readonly OnnxEmbeddingProvider _embeddingProvider;

    public EmbeddingController(OnnxEmbeddingProvider embeddingProvider)
    {
        _embeddingProvider = embeddingProvider;
    }

    [HttpPost]
    [Route("embed")]
    public async Task<IActionResult> CreateEmbeddings([FromBody] EmbedRequest embedRequest)
    {
        if (embedRequest == null || embedRequest.Texts == null || embedRequest.Texts.Count == 0)
        {
            return BadRequest(new { error = "texts must contain at least one entry" });
        }

        // modelName wird aktuell nicht von ONNX verwendet,
        // aber für API-Kompatibilität behalten wir den Parameter.
        string modelName;
        if (!string.IsNullOrWhiteSpace(embedRequest.Model))
        {
            modelName = embedRequest.Model;
        }
        else
        {
            modelName = "onnx-embedding-model";
        }

        EmbedResponse embedResponse = await _embeddingProvider.CreateEmbeddingsAsync(
            embedRequest.Texts,
            modelName);

        return Ok(embedResponse);
    }
}