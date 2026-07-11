using Microsoft.AspNetCore.Mvc;
using PassXYZ.Server.DTOs.Dashboard;
using PassXYZ.Server.Services;

namespace PassXYZ.Server.Controllers;

[ApiController]
[Route("api/v1")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    private string? GetUserId()
    {
        return HttpContext.Items["Username"] as string;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboards()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var dashboards = await _dashboardService.GetDashboardsAsync(userId);
        return Ok(dashboards);
    }

    [HttpGet("dashboard/{dashboard_id}")]
    public async Task<IActionResult> GetDashboard(string dashboard_id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var dashboard = await _dashboardService.GetDashboardAsync(userId, dashboard_id);
        if (dashboard == null) return NotFound();

        return Ok(dashboard);
    }

    [HttpPost("dashboard")]
    public async Task<IActionResult> CreateDashboard([FromBody] DashboardCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var dashboard = await _dashboardService.CreateDashboardAsync(userId, request);
        return Ok(dashboard);
    }

    [HttpPut("dashboard/{dashboard_id}")]
    public async Task<IActionResult> UpdateDashboard(string dashboard_id, [FromBody] DashboardUpdateRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var dashboard = await _dashboardService.UpdateDashboardAsync(userId, dashboard_id, request);
        if (dashboard == null) return NotFound();

        return Ok(dashboard);
    }

    [HttpDelete("dashboard/{dashboard_id}")]
    public async Task<IActionResult> DeleteDashboard(string dashboard_id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _dashboardService.DeleteDashboardAsync(userId, dashboard_id);
        return Ok();
    }

    [HttpPost("dashboard/template/{template_name}")]
    public async Task<IActionResult> CreateFromTemplate(string template_name)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var dashboard = await _dashboardService.CreateFromTemplateAsync(userId, template_name);
        return Ok(dashboard);
    }

    [HttpGet("dashboard/{dashboard_id}/widgets")]
    public async Task<IActionResult> GetWidgets(string dashboard_id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var widgets = await _dashboardService.GetWidgetsAsync(userId, dashboard_id);
        return Ok(widgets);
    }

    [HttpPost("dashboard/{dashboard_id}/widgets")]
    public async Task<IActionResult> AddWidget(string dashboard_id, [FromBody] WidgetCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var widget = await _dashboardService.AddWidgetAsync(userId, dashboard_id, request);
        return Ok(widget);
    }

    [HttpPut("dashboard/{dashboard_id}/widgets/{widget_id}")]
    public async Task<IActionResult> UpdateWidget(string dashboard_id, string widget_id, [FromBody] WidgetUpdateRequest request)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var widget = await _dashboardService.UpdateWidgetAsync(userId, dashboard_id, widget_id, request);
        if (widget == null) return NotFound();

        return Ok(widget);
    }

    [HttpDelete("dashboard/{dashboard_id}/widgets/{widget_id}")]
    public async Task<IActionResult> DeleteWidget(string dashboard_id, string widget_id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _dashboardService.DeleteWidgetAsync(userId, dashboard_id, widget_id);
        return Ok();
    }
}