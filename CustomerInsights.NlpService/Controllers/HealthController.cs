using Microsoft.AspNetCore.Mvc;

namespace CustomerInsights.NlpService.Controllers;

[ApiController]
[Route("")]
public sealed class HealthController : ControllerBase
{
    [HttpGet("healthz")]
    public IActionResult Health()
    {
        return Ok(new { status = "ok" });
    }

    [HttpGet("readyz")]
    public IActionResult Ready()
    {
        return Ok(new { models = "loaded" });
    }

    [HttpGet]
    public IActionResult Root() 
    {
        return Ok(new { name = "CustomerInsights NLP", status = "ok" });
    } 
}
