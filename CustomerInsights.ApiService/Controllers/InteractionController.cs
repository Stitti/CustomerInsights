using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.ApiService.Services;
using CustomerInsights.Models;
using CustomerInsights.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CustomerInsights.ApiService.Controllers;

[ApiController]
[Route("api/v1/interactions")]
[Produces("application/json")]
public sealed class InteractionController : ControllerBase
{
    private readonly InteractionService _interactionService;
    private readonly ILogger<InteractionController> _logger;

    public InteractionController(InteractionService interactionService, ILogger<InteractionController> logger)
    {
        _interactionService = interactionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        IEnumerable<InteractionListDto> interactions = await _interactionService.GetAllInteractionsAsync();
        return Ok(interactions);
    }

    [HttpGet, Route("{id}")]
    public async Task<IActionResult> GetInteractionById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid interaction id");
        }

        InteractionDto? interaction = await _interactionService.GetInteractionByIdAsync(id);
        return interaction == null ? BadRequest($"Interaction with id {id} was not found") : Ok(interaction);
    }

    [HttpGet("top-channels")]
    public async Task<ActionResult<IEnumerable<ChannelCountResponse>>> GetTopChannels([FromQuery] TimeInterval period = TimeInterval.ThisMonth)
    {
        try
        {
            IReadOnlyList<ChannelCount> rows = await _interactionService.GetTopChannelsAsync(period);

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

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateInteraction(Guid id, [FromBody] UpdateInteractionRequest request)
    {
        Guid tenantId = Guid.Empty;

        if (id == Guid.Empty)
        {
            return BadRequest("Invalid ID");
        }

        if (request == null)
        {
            return BadRequest("Invalid request");
        }

        try
        {
            _logger.LogInformation("Patching interaction {InteractionId} for tenant {TenantId}", id, tenantId);
            bool success = await _interactionService.PatchAsync(id, request);
            return success ? Accepted() : BadRequest();

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
            _logger.LogError(ex, "Failed to patch interaction {InteractionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Patching Failed",
                Detail = "An error occurred while patching the interaction",
                Status = StatusCodes.Status500InternalServerError
            });
        }

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        Guid tenantId = Guid.Empty;

        if (id == Guid.Empty)
            return BadRequest("Invalid interaction id");

        try
        {
            _logger.LogInformation("Deleting interaction {InteractionId} for tenant {TenantId}", id, tenantId);
            bool success = await _interactionService.DeleteAsync(id);
            return success ? Accepted() : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete interaction {InteractionId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Deletion Failed",
                Detail = "An error occurred while deleting the interaction",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}