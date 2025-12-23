using FAP.Admin.Web.Services;
using FAP.Share.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FAP.Admin.Web.Controllers;

/// <summary>
/// Controller for Permission Management operations
/// </summary>
[Route("permissionmanagement")]
public class PermissionManagementController : Controller
{
    private readonly ILogger<PermissionManagementController> _logger;
    private readonly IUserManagementApiClient _apiClient;

    public PermissionManagementController(
        ILogger<PermissionManagementController> logger,
        IUserManagementApiClient apiClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    /// <summary>
    /// Danh sách permissions với filter
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(string? module = null, bool? isActive = null)
    {
        try
        {
            var permissions = await _apiClient.GetPermissionsAsync(module, isActive);
            
            // Lấy danh sách modules để hiển thị filter
            var allPermissions = await _apiClient.GetPermissionsAsync();
            var modules = allPermissions.Select(p => p.Module).Distinct().OrderBy(m => m).ToList();

            ViewBag.Modules = modules;
            ViewBag.SelectedModule = module;
            ViewBag.IsActive = isActive;

            // Nhóm permissions theo module
            var permissionsByModule = permissions.GroupBy(p => p.Module).ToList();

            return View(permissionsByModule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading permissions");
            TempData["Error"] = "Không thể tải danh sách quyền.";
            return View(new List<IGrouping<string, PermissionDto>>());
        }
    }
}

