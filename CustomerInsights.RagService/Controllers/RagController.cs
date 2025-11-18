using CustomerInsights.RagService.Models;
using CustomerInsights.RagService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CustomerInsights.RagService.Controllers
{
    [ApiController]
    [Route("rag")]
    public class RagController : ControllerBase
    {
        private readonly RagQueryService _ragQueryService;

        public RagController(RagQueryService ragQueryService)
        {
            _ragQueryService = ragQueryService;
        }

        [HttpPost]
        [Route("query")]
        public async Task<IActionResult> Query([FromBody] RagRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest(new { error = "Question must be provided." });
            }

            RagResponse response = await _ragQueryService.QueryAsync(request);
            return Ok(response);
        }
    }
}