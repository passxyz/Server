using Microsoft.EntityFrameworkCore;
using PassXYZ.Server.Data;
using PassXYZ.Server.DTOs.Dashboard;
using PassXYZ.Server.Models;
using System.Text.Json;

namespace PassXYZ.Server.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardDbContextFactory _dbContextFactory;

    public DashboardService(IDashboardDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    private DashboardDbContext GetDbContext(string userId)
    {
        return _dbContextFactory.Create(userId);
    }

    public async Task<IEnumerable<DashboardDto>> GetDashboardsAsync(string userId)
    {
        using var db = GetDbContext(userId);

        var dashboards = await db.Dashboards
            .Where(d => d.UserId == userId)
            .ToListAsync();

        var dashboardIds = dashboards.Select(d => d.Id).ToList();
        var widgets = await db.Widgets
            .Where(w => dashboardIds.Contains(w.DashboardId))
            .ToListAsync();

        var widgetLookup = widgets.GroupBy(w => w.DashboardId).ToDictionary(g => g.Key, g => g.Select(ConvertToWidgetDto).ToList());

        return dashboards.Select(dashboard =>
        {
            var dto = ConvertToDto(dashboard);
            if (widgetLookup.TryGetValue(dashboard.Id, out var dashboardWidgets))
            {
                dto.Widgets = dashboardWidgets;
            }
            return dto;
        });
    }

    public async Task<DashboardDto> GetDashboardAsync(string userId, string dashboardId)
    {
        using var db = GetDbContext(userId);

        var dashboard = await db.Dashboards
            .FirstOrDefaultAsync(d => d.UserId == userId && d.Id == dashboardId);

        if (dashboard == null)
        {
            return null!;
        }

        var widgets = await db.Widgets
            .Where(w => w.DashboardId == dashboardId)
            .ToListAsync();

        var dto = ConvertToDto(dashboard);
        dto.Widgets = widgets.Select(ConvertToWidgetDto).ToList();

        return dto;
    }

    public async Task<DashboardDto> CreateDashboardAsync(string userId, DashboardCreateRequest request)
    {
        using var db = GetDbContext(userId);

        var now = DateTime.UtcNow.ToString("o");

        var dashboard = new Dashboard
        {
            Id = request.Id,
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            Widgets = request.Widgets != null ? JsonSerializer.Serialize(request.Widgets) : null,
            Tabs = request.Tabs != null ? JsonSerializer.Serialize(request.Tabs) : null,
            Groups = request.Groups != null ? JsonSerializer.Serialize(request.Groups) : null,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Dashboards.Add(dashboard);
        await db.SaveChangesAsync();

        return ConvertToDto(dashboard);
    }

    public async Task<DashboardDto> UpdateDashboardAsync(string userId, string dashboardId, DashboardUpdateRequest request)
    {
        using var db = GetDbContext(userId);

        var dashboard = await db.Dashboards
            .FirstOrDefaultAsync(d => d.UserId == userId && d.Id == dashboardId);

        if (dashboard == null)
        {
            return null!;
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            dashboard.Name = request.Name;
        }

        if (request.Description != null)
        {
            dashboard.Description = request.Description;
        }

        if (request.Widgets != null)
        {
            dashboard.Widgets = JsonSerializer.Serialize(request.Widgets);
        }

        if (request.Tabs != null)
        {
            dashboard.Tabs = JsonSerializer.Serialize(request.Tabs);
        }

        if (request.Groups != null)
        {
            dashboard.Groups = JsonSerializer.Serialize(request.Groups);
        }

        dashboard.UpdatedAt = DateTime.UtcNow.ToString("o");

        await db.SaveChangesAsync();

        return ConvertToDto(dashboard);
    }

    public async Task DeleteDashboardAsync(string userId, string dashboardId)
    {
        using var db = GetDbContext(userId);

        var dashboard = await db.Dashboards
            .FirstOrDefaultAsync(d => d.UserId == userId && d.Id == dashboardId);

        if (dashboard != null)
        {
            db.Dashboards.Remove(dashboard);
            await db.SaveChangesAsync();
        }
    }

    public async Task<DashboardDto> CreateFromTemplateAsync(string userId, string templateName)
    {
        var request = new DashboardCreateRequest
        {
            Id = Guid.NewGuid().ToString(),
            Name = $"{templateName} Dashboard",
            Description = $"Created from {templateName} template",
            Widgets = new object[] { },
            Tabs = new object[] { },
            Groups = new object[] { }
        };

        return await CreateDashboardAsync(userId, request);
    }

    public async Task<IEnumerable<WidgetDto>> GetWidgetsAsync(string userId, string dashboardId)
    {
        using var db = GetDbContext(userId);

        var widgets = await db.Widgets
            .Where(w => w.DashboardId == dashboardId)
            .ToListAsync();

        return widgets.Select(ConvertToWidgetDto);
    }

    public async Task<WidgetDto> AddWidgetAsync(string userId, string dashboardId, WidgetCreateRequest request)
    {
        using var db = GetDbContext(userId);

        var now = DateTime.UtcNow.ToString("o");

        var widget = new Widget
        {
            Id = request.Id,
            DashboardId = dashboardId,
            Type = request.Type,
            Title = request.Title,
            Position = JsonSerializer.Serialize(request.Position),
            Data = request.Data != null ? JsonSerializer.Serialize(request.Data) : null,
            CreatedAt = now,
            UpdatedAt = now
        };

        db.Widgets.Add(widget);
        await db.SaveChangesAsync();

        return ConvertToWidgetDto(widget);
    }

    public async Task<WidgetDto> UpdateWidgetAsync(string userId, string dashboardId, string widgetId, WidgetUpdateRequest request)
    {
        using var db = GetDbContext(userId);

        var widget = await db.Widgets
            .FirstOrDefaultAsync(w => w.DashboardId == dashboardId && w.Id == widgetId);

        if (widget == null)
        {
            return null!;
        }

        if (!string.IsNullOrEmpty(request.Title))
        {
            widget.Title = request.Title;
        }

        if (request.Position != null)
        {
            widget.Position = JsonSerializer.Serialize(request.Position);
        }

        if (request.Data != null)
        {
            widget.Data = JsonSerializer.Serialize(request.Data);
        }

        widget.UpdatedAt = DateTime.UtcNow.ToString("o");

        await db.SaveChangesAsync();

        return ConvertToWidgetDto(widget);
    }

    public async Task DeleteWidgetAsync(string userId, string dashboardId, string widgetId)
    {
        using var db = GetDbContext(userId);

        var widget = await db.Widgets
            .FirstOrDefaultAsync(w => w.DashboardId == dashboardId && w.Id == widgetId);

        if (widget != null)
        {
            db.Widgets.Remove(widget);
            await db.SaveChangesAsync();
        }
    }

    private DashboardDto ConvertToDto(Dashboard dashboard)
    {
        return new DashboardDto
        {
            Id = dashboard.Id,
            Name = dashboard.Name,
            Description = dashboard.Description,
            Widgets = !string.IsNullOrEmpty(dashboard.Widgets) ? JsonSerializer.Deserialize<object>(dashboard.Widgets) : null,
            Tabs = !string.IsNullOrEmpty(dashboard.Tabs) ? JsonSerializer.Deserialize<object>(dashboard.Tabs) : null,
            Groups = !string.IsNullOrEmpty(dashboard.Groups) ? JsonSerializer.Deserialize<object>(dashboard.Groups) : null,
            CreatedAt = dashboard.CreatedAt,
            UpdatedAt = dashboard.UpdatedAt
        };
    }

    private WidgetDto ConvertToWidgetDto(Widget widget)
    {
        return new WidgetDto
        {
            Id = widget.Id,
            Type = widget.Type,
            Title = widget.Title,
            Position = JsonSerializer.Deserialize<object>(widget.Position)!,
            Data = !string.IsNullOrEmpty(widget.Data) ? JsonSerializer.Deserialize<object>(widget.Data) : null
        };
    }
}