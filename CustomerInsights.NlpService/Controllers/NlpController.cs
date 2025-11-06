using Microsoft.AspNetCore.Mvc;
using CustomerInsights.NlpService.Runtime;
using CustomerInsights.Base.Models.Responses;
using CustomerInsights.Base.Models.Requests;

namespace CustomerInsights.NlpService.Controllers;

[ApiController]
[Route("api/nlp")]
[Produces("application/json")]
public sealed class NlpController : ControllerBase
{
    private readonly TextAnalyzer _analyzer;
    private readonly ILogger<NlpController> _logger;

    public NlpController(TextAnalyzer analyzer, ILogger<NlpController> logger)
    {
        _analyzer = analyzer;
        _logger = logger;
    }

    /// <summary>
    /// Analysiert einen einzelnen Text (Sprache → optional Übersetzen → Sentiment/Aspekte/Embedding …).
    /// </summary>
    [HttpPost("analyze")]
    public IActionResult Analyze([FromBody] NlpRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Text))
            return BadRequest("Field 'text' is required.");

        try
        {
            NlpResponse res = _analyzer.Analyze(request.Text);
            return Ok(res);
        }
        catch (OperationCanceledException)
        {
            return Problem(title: "Canceled", statusCode: StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Analyze failed");
            return Problem(title: "NLP error", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}