using CustomerInsights.ApiService.Models.Contracts;
using CustomerInsights.ApiService.Models.DTOs;
using CustomerInsights.ApiService.Services;
using CustomerInsights.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerInsights.ApiService.Controllers
{
    [ApiController]
    [Route("api/v1/apikeys")]
    [Produces("application/json")]
    public sealed class ApiKeyController : ControllerBase
    {
        private readonly ApiKeyService _apiKeyService;
        private readonly ILogger<ApiKeyController> _logger;

        public ApiKeyController(ApiKeyService apiKeyService, ILogger<ApiKeyController> logger)
        {
            _apiKeyService = apiKeyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            Guid tenantId = Guid.Empty;
            try
            {
                _logger.LogInformation("Fetching all api keys for tenant {TenantId}", tenantId);
                IEnumerable<ApiKeyDto> apiKeys = await _apiKeyService.GetAllApiKeysAsync();
                return Ok(apiKeys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch api keys for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Fetch Failed",
                    Detail = "An error occurred while fetching api keys",
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
                _logger.LogInformation("Fetching all api keys {ApiKeyId} for tenant {TenantId}", id, tenantId);
                ApiKeyDto? apiKey = await _apiKeyService.GetApiKeyByIdAsync(id);
                return Ok(apiKey ?? new ApiKeyDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch api key {ApiKeyId} for tenant {TenantId}", id, tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Fetch Failed",
                    Detail = "An error occurred while fetching the api keys",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest request)
        {
            Guid tenantId = Guid.Empty;
            if (request == null)
                return BadRequest("Invalid api key request");

            try
            {
                _logger.LogInformation("Creating api key for tenant {TenantId}", tenantId);
                string? token = await _apiKeyService.CreateAsync(request);
                if (string.IsNullOrWhiteSpace(token))
                    return BadRequest("Failed to create api keys");

                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create api key for tenant {TenantId}", tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Creation Failed",
                    Detail = "An error occurred while creating the api key",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(Guid id, [FromBody] UpdateApiKeyRequest request)
        {
            Guid tenantId = Guid.Empty;

            if (id == Guid.Empty)
                return BadRequest("Invalid api key id");

            try
            {
                _logger.LogInformation("Patching api key {ApiKeyId} for tenant {TenantId}", id, tenantId);
                bool success = await _apiKeyService.PatchAsync(id, request);
                return success ? Accepted() : BadRequest();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid api key data");
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid Data",
                    Detail = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to patch api key {ApiKeyId} for teant {TenantId}", id, tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Patching Failed",
                    Detail = "An error occurred while patching the api key",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPatch("{id}/revoke")]
        public async Task<IActionResult> Revoke(Guid id)
        {
            Guid tenantId = Guid.Empty;
            if (id == Guid.Empty)
                return BadRequest("Invalid api key id");

            try
            {
                _logger.LogInformation("Revoking api key {ApiKeyId} for tenant {TenantId}", id, tenantId);
                bool success = await _apiKeyService.RevokeAsync(id);
                return success ? Accepted() : BadRequest($"Failed to revoke api key with id {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke api key {ApiKeyId} for tenant {TenantId}", id, tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Revoking Failed",
                    Detail = "An error occurred while revoking the api key",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPatch("{id}/renew")]
        public async Task<IActionResult> Renew(Guid id)
        {
            Guid tenantId = Guid.Empty;
            if (id == Guid.Empty)
                return BadRequest("Invalid api key id");

            try
            {
                _logger.LogInformation("Renewing api key {ApiKeyId} for tenant {TenantId}", id, tenantId);
                string? token = await _apiKeyService.RenewAsync(id);
                if (string.IsNullOrWhiteSpace(token))
                    return BadRequest($"Failed to renew api key with id {id}");

                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to renew api key {ApiKeyId} for tenant {TenantId}", id, tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Renewal Failed",
                    Detail = "An error occurred while renewing the api key",
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
                _logger.LogInformation("Deleting api keys {ApiKeyId} for tenant {TenantId}", id, tenantId);
                bool success = await _apiKeyService.DeleteAsync(id);
                return success ? Accepted() : BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete api keys {ApiKeyId} for tenant {TenantId}", id, tenantId);
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Deletion Failed",
                    Detail = "An error occurred while deleting the api keys",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
