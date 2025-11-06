using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Services;
using CustomerInsights.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CustomerInsights.ApiService.Controllers;

[ApiController]
[Route("api/v1/interactions")]
[Produces("application/json")]
public sealed class IngestController : ControllerBase
{
    private readonly InteractionService _interactionService;
    private readonly ILogger<IngestController> _logger;

    public IngestController(InteractionService interactionService, ILogger<IngestController> logger)
    {
        _interactionService = interactionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<Interaction> interactions = await _interactionService.GetAllInteractionsAsync();
        return Ok(interactions);
    }

    [HttpGet, Route("{id}")]
    public async Task<IActionResult> GetInteractionById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid interaction id");
        }

        Interaction? interaction = await _interactionService.GetInteractionByIdAsync(id);
        return interaction == null ? BadRequest($"Interaction with id {id} was not found") : Ok(interaction);
    }

    [HttpGet("top-channels")]
    public async Task<ActionResult<IEnumerable<ChannelCountResponse>>> GetTopChannels([FromQuery] Period period = Period.LastMonth, [FromQuery] DateTimeOffset? fromUtc = null, [FromQuery] DateTimeOffset? toUtc = null)
    {
        try
        {
            IReadOnlyList<ChannelCount> rows = await _interactionService.GetTopChannelsAsync(period, DateTimeOffset.UtcNow, fromUtc, toUtc);

            IEnumerable<ChannelCountResponse> result = rows
                .Select(r => new ChannelCountResponse
                {
                    Channel = r.Channel,
                    ChannelName = r.Channel.ToString(),
                    InteractionCount = r.InteractionCount
                });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<IActionResult> IngestInteraction([FromBody] IngestInteractionRequest request)
    {
        Guid tenantId = Guid.Empty;
        if (ModelState.IsValid == false)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        try
        {
            _logger.LogInformation("Ingesting interaction for tenant {TenantId}, account {AccountId}, channel {Channel}", tenantId, request.AccountId, request.Channel);
            await _interactionService.IngestAsync(tenantId, request);
            return Accepted();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid interaction data");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Data",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ingest interaction");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Ingestion Failed",
                Detail = "An error occurred while processing the interaction",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpPost("batch")]
    public async Task<IActionResult> IngestBatch([FromBody] IngestInteractionRequest[] requests)
    {
        Guid tenantId = Guid.Empty;

        if (requests == null || requests.Length == 0)
        {
            return BadRequest("Batch must contain at least one interaction");
        }

        if (requests.Length > 1000)
        {
            return BadRequest("Batch size exceeds maximum of 1000 interactions");
        }

        try
        {
            _logger.LogInformation("Ingesting batch of {Count} interactions for tenant {TenantId}", requests.Length, tenantId);

            BatchIngestResponse response = await _interactionService.IngestBatchAsync(tenantId, requests);

            return Accepted(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ingest batch");
            return StatusCode(500, "Batch ingestion failed");
        }
    }
}

public sealed class AnalysisResults
{
    public double SentimentScore { get; set; }
    public double Urgency { get; set; }
    public double Severity { get; set; }
    public string[] Aspects { get; set; } = Array.Empty<string>();
    public double SatisfactionIndex { get; set; }
}