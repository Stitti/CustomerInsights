using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerInsights.ApiService.Controllers;

[ApiController]
[Route("api/v1/accounts")]
[Produces("application/json")]
public sealed class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(AccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Guid tenantId = Guid.Empty;
        try
        {
            _logger.LogInformation("Fetching all accounts for tenant {TenantId}", tenantId);
            await _accountService.GetAllAccountsAsync();
            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch accounts");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Fetch Failed",
                Detail = "An error occurred while fetching accounts",
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
            _logger.LogInformation("Fetching all account {ContactId} for tenant {TenantId}", id, tenantId);
            await _accountService.GetAccountById(id);
            return Accepted();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch account");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Fetch Failed",
                Detail = "An error occurred while fetching the account",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact([FromBody] CreateAccountRequest request)
    {
        Guid tenantId = Guid.Empty;
        if (ModelState.IsValid == false)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        try
        {
            _logger.LogInformation("Creating account for tenant {TenantId}", tenantId);
            await _accountService.CreateAccountAsync(request);
            return Accepted();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid account data");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Data",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create account");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Creation Failed",
                Detail = "An error occurred while processing the account",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}