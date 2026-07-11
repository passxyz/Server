using PassXYZ.Server.DTOs.Dashboard;

namespace PassXYZ.Server.Services;

public interface IDashboardService
{
    Task<IEnumerable<DashboardDto>> GetDashboardsAsync(string userId);
    Task<DashboardDto> GetDashboardAsync(string userId, string dashboardId);
    Task<DashboardDto> CreateDashboardAsync(string userId, DashboardCreateRequest request);
    Task<DashboardDto> UpdateDashboardAsync(string userId, string dashboardId, DashboardUpdateRequest request);
    Task DeleteDashboardAsync(string userId, string dashboardId);
    Task<DashboardDto> CreateFromTemplateAsync(string userId, string templateName);
    Task<IEnumerable<WidgetDto>> GetWidgetsAsync(string userId, string dashboardId);
    Task<WidgetDto> AddWidgetAsync(string userId, string dashboardId, WidgetCreateRequest request);
    Task<WidgetDto> UpdateWidgetAsync(string userId, string dashboardId, string widgetId, WidgetUpdateRequest request);
    Task DeleteWidgetAsync(string userId, string dashboardId, string widgetId);
}