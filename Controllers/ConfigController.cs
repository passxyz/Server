using Microsoft.AspNetCore.Mvc;

namespace PassXYZ.Server.Controllers;

[ApiController]
[Route("api")]
public class ConfigController : ControllerBase
{
    [HttpGet("apps.json")]
    [Produces("application/json")]
    public IActionResult GetApps()
    {
        return Ok(new object[] { });
    }

    [HttpGet("agents.json")]
    [Produces("application/json")]
    public IActionResult GetAgents()
    {
        return Ok(new object[] { });
    }

    [HttpGet("widgets.json")]
    [Produces("application/json")]
    public IActionResult GetWidgets()
    {
        return Ok(new object[] { });
    }
}