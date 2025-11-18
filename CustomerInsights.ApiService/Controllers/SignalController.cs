using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerInsights.ApiService.Controllers;

[ApiController]
[Route("api/v1/signals")]
[Produces("application/json")]
public sealed class SignalController : ControllerBase
{
    private readonly SignalService _signalService;
    private readonly ILogger<SignalController> _logger;

    public SignalController(SignalService signalService, ILogger<SignalController> logger)
    {
        _signalService = signalService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Guid tenantId = Guid.Empty;
        try
        {
            _logger.LogInformation("Fetching all signals for tenant {TenantId}", tenantId);
            IEnumerable<SignalDto> signals = await _signalService.GetAll();
            return Ok(signals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch signals");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Fetch Failed",
                Detail = "An error occurred while fetching signals",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpGet, Route("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        Guid tenantId = Guid.Empty;
        try
        {
            _logger.LogInformation("Fetching signal {SignalId} for tenant {TenantId}", id, tenantId);
            SignalDto? signal = await _signalService.GetById(id);
            return Ok(signal ?? new SignalDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch signal");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Fetch Failed",
                Detail = "An error occurred while fetching the signal",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}