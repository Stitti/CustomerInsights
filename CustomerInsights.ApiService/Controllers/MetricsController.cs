using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Models.Enums;
using CustomerInsights.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerInsights.ApiService.Controllers;

[ApiController]
[Route("api/v1/metrics")]
[Produces("application/json")]
public sealed class MetricsController : ControllerBase
{
    private readonly MetricsService _metricsService;
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(MetricsService metricsService, ILogger<MetricsController> logger)
    {
        _metricsService = metricsService;
        _logger = logger;
    }

    /// <summary>
    /// Holt Tenant-Metriken mit optionalem Zeitintervall und Trends.
    /// </summary>
    /// <param name="interval">Zeitintervall (ThisWeek, ThisMonth, ThisYear, LastWeek, LastMonth, LastYear). Default: ThisMonth</param>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TimeInterval interval = TimeInterval.ThisMonth)
    {
        Guid tenantId = new Guid("263049a9-4b66-47ab-b144-576958283f7e");
        try
        {
            _logger.LogInformation("Fetching metrics for tenant {TenantId} with interval {Interval}", tenantId, interval);
            MetricsDto? metrics = await _metricsService.GetTenantMetrics(tenantId, interval);
            return Ok(metrics ?? new MetricsDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch metrics for interval {Interval}", interval);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Fetch Failed",
                Detail = "An error occurred while fetching the metrics",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    /// <summary>
    /// Holt Account-Metriken mit optionalem Zeitintervall und Trends.
    /// </summary>
    /// <param name="id">Account ID</param>
    /// <param name="interval">Zeitintervall (ThisWeek, ThisMonth, ThisYear, LastWeek, LastMonth, LastYear). Default: ThisMonth</param>
    [HttpGet, Route("account/{id}")]
    public async Task<IActionResult> GetById(
        Guid id,
        [FromQuery] TimeInterval interval = TimeInterval.ThisMonth)
    {
        Guid tenantId = new Guid("263049a9-4b66-47ab-b144-576958283f7e");
        try
        {
            _logger.LogInformation(
                "Fetching metrics for account {AccountId} on tenant {TenantId} with interval {Interval}",
                id,
                tenantId,
                interval);

            MetricsDto? metrics = await _metricsService.GetAccountMetrics(tenantId, id, interval);

            if (metrics == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Account Not Found",
                    Detail = $"Account with ID {id} was not found",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch metrics for account {AccountId} with interval {Interval}", id, interval);

            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Fetch Failed",
                Detail = "An error occurred while fetching the metrics",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}