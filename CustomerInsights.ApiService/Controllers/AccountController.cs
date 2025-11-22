using CustomerInsights.ApiService.Models;
using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Services;
using CustomerInsights.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            IEnumerable<AccountListDto> accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch accounts for tenant {TenantId}", tenantId);
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
            AccountDto? account = await _accountService.GetAccountById(id);
            return Ok(account ?? new AccountDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch account for tenant {TenantId}", tenantId);
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
            _logger.LogError(ex, "Failed to create account for tenant {TenantId}", tenantId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Creation Failed",
                Detail = "An error occurred while processing the account",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateAccountRequest request)
    {
        Guid tenantId = Guid.Empty;

        if (id == Guid.Empty)
            return BadRequest("Invalid account id");

        try
        {
            _logger.LogInformation("Patching account {AccountId} for tenant {TenantId}", id, tenantId);
            bool success = await _accountService.PatchAsync(id, request);
            return success ? Accepted() : BadRequest();
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
            _logger.LogError(ex, "Failed to patch account {AccountId} for tenant {TenantId}", id, tenantId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Patching Failed",
                Detail = "An error occurred while patching the account",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        Guid tenantId = Guid.Empty;

        if (id == Guid.Empty)
            return BadRequest("Invalid account id");

        try
        {
            _logger.LogInformation("Deleting account {AccountId} for tenant {TenantId}", id, tenantId);
            bool success = await _accountService.DeleteAsync(id);
            return success ? Accepted() : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete account {AccountId} for tenant {TenantId}", id, tenantId);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Deletion Failed",
                Detail = "An error occurred while deleting the account",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}