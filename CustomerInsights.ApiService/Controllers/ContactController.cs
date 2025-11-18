using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Services;
using CustomerInsights.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerInsights.ApiService.Controllers;

[ApiController]
[Route("api/v1/contacts")]
[Produces("application/json")]
public sealed class ContactController : ControllerBase
{
    private readonly ContactService _contactService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(ContactService contactService, ILogger<ContactController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Guid tenantId = Guid.Empty;
        try
        {
            _logger.LogInformation("Fetching all contacts for tenant {TenantId}", tenantId);
            IEnumerable<ContactListDto> contacts = await _contactService.GetAllContactsAsync();
            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch contacts");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Fetch Failed",
                Detail = "An error occurred while fetching contacts",
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
            _logger.LogInformation("Fetching all contact {ContactId} for tenant {TenantId}", id, tenantId);
            ContactDto contact = await _contactService.GetContactById(id);
            return Ok(contact ?? new ContactDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch contact");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Fetch Failed",
                Detail = "An error occurred while fetching the contact",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact([FromBody] CreateContactRequest request)
    {
        Guid tenantId = Guid.Empty;
        if (ModelState.IsValid == false)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        try
        {
            _logger.LogInformation("Creating contact for tenant {TenantId}, account {AccountId}", tenantId, request.AccountId);
            await _contactService.CreateContactAsync(request);
            return Accepted();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid contact data");
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Data",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create contact");
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Creation Failed",
                Detail = "An error occurred while processing the contact",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateContactRequest request)
    {
        Guid tenantId = Guid.Empty;

        if (id == Guid.Empty)
            return BadRequest("Invalid account id");

        try
        {
            _logger.LogInformation("Patching account {AccountId} for tenant {TenantId}", id, tenantId);
            bool success = await _contactService.PatchAsync(id, request);
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
            _logger.LogError(ex, "Failed to patch account {AccountId}", id);
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
            return BadRequest("Invalid contact id");

        try
        {
            _logger.LogInformation("Deleting contact {ContactId} for tenant {TenantId}", id, tenantId);
            bool success = await _contactService.DeleteAsync(id);
            return success ? Accepted() : BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete contact {ContactId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Title = "Deletion Failed",
                Detail = "An error occurred while deleting the contact",
                Status = StatusCodes.Status500InternalServerError
            });
        }
    }
}