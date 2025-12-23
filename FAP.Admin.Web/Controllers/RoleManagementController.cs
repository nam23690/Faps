using FAP.Share.Dtos;
using FAP.Admin.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FAP.Admin.Web.Controllers;

/// <summary>
/// Controller for Role Management operations
/// </summary>
[Route("rolemanagement")]
public class RoleManagementController : Controller
{
    private readonly ILogger<RoleManagementController> _logger;
    private readonly IUserManagementApiClient _apiClient;

    public RoleManagementController(
        ILogger<RoleManagementController> logger,
        IUserManagementApiClient apiClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    /// <summary>
    /// Danh sách roles
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var roles = await _apiClient.GetRolesAsync();
            return View(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading roles");
            TempData["Error"] = "Không thể tải danh sách vai trò.";
            return View(new List<RoleDto>());
        }
    }

    /// <summary>
    /// Form tạo vai trò mới
    /// </summary>
    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new CreateRoleViewModel());
    }

    /// <summary>
    /// Xử lý tạo vai trò mới
    /// </summary>
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new CreateRoleRequest
            {
                Name = model.Name.Trim()
            };

            await _apiClient.CreateRoleAsync(request);
            TempData["Success"] = "Tạo vai trò thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error creating role");
            ModelState.AddModelError(string.Empty, "Không thể tạo vai trò. Vui lòng thử lại.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating role");
            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra. Vui lòng thử lại.");
            return View(model);
        }
    }

    /// <summary>
    /// Form chỉnh sửa vai trò
    /// </summary>
    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var roles = await _apiClient.GetRolesAsync();
            var role = roles.FirstOrDefault(r => r.Id == id);

            if (role == null)
            {
                TempData["Error"] = "Không tìm thấy vai trò.";
                return RedirectToAction(nameof(Index));
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                Name = role.Name
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading role for edit {RoleId}", id);
            TempData["Error"] = "Không thể tải thông tin vai trò.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Xử lý chỉnh sửa vai trò
    /// </summary>
    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditRoleViewModel model)
    {
        if (id != model.Id)
        {
            TempData["Error"] = "ID vai trò không khớp.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new UpdateRoleRequest
            {
                Name = model.Name.Trim()
            };

            await _apiClient.UpdateRoleAsync(id, request);
            TempData["Success"] = "Cập nhật vai trò thành công.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            ModelState.AddModelError(string.Empty, "Không thể cập nhật vai trò. Vui lòng thử lại.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating role {RoleId}", id);
            ModelState.AddModelError(string.Empty, "Có lỗi xảy ra. Vui lòng thử lại.");
            return View(model);
        }
    }

    /// <summary>
    /// Chi tiết role - hiển thị users và permissions của role
    /// </summary>
    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var roles = await _apiClient.GetRolesAsync();
            var role = roles.FirstOrDefault(r => r.Id == id);
            
            if (role == null)
            {
                TempData["Error"] = "Không tìm thấy vai trò.";
                return RedirectToAction(nameof(Index));
            }

            // Lấy danh sách users có role này
            var allUsers = await _apiClient.GetUsersAsync();
            var usersWithRole = allUsers.Where(u => u.Roles.Contains(role.Name)).ToList();

            // Lấy permissions của role
            var rolePermissions = await _apiClient.GetRolePermissionsAsync(id);
            
            // Lấy tất cả permissions để hiển thị
            var allPermissions = await _apiClient.GetPermissionsAsync(isActive: true);

            ViewBag.UsersWithRole = usersWithRole;
            ViewBag.RolePermissions = rolePermissions;
            ViewBag.AllPermissions = allPermissions;

            return View(role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading role details for {RoleId}", id);
            TempData["Error"] = "Không thể tải thông tin vai trò.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Form gán permissions cho role
    /// </summary>
    [HttpGet("assign-permissions/{roleId}")]
    public async Task<IActionResult> AssignPermissions(string roleId)
    {
        try
        {
            var roles = await _apiClient.GetRolesAsync();
            var role = roles.FirstOrDefault(r => r.Id == roleId);
            
            if (role == null)
            {
                TempData["Error"] = "Không tìm thấy vai trò.";
                return RedirectToAction(nameof(Index));
            }

            var permissions = await _apiClient.GetPermissionsAsync(isActive: true);
            var rolePermissions = await _apiClient.GetRolePermissionsAsync(roleId);

            var model = new AssignPermissionsToRoleViewModel
            {
                RoleId = roleId,
                RoleName = role.Name,
                CurrentPermissions = rolePermissions,
                AvailablePermissions = permissions
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading assign permissions for role {RoleId}", roleId);
            TempData["Error"] = "Không thể tải thông tin.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Xử lý gán permissions cho role
    /// </summary>
    [HttpPost("assign-permissions/{roleId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignPermissions(string roleId, AssignPermissionsToRoleViewModel model)
    {
        if (roleId != model.RoleId)
        {
            TempData["Error"] = "ID không khớp.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var request = new AssignPermissionToRoleRequest
            {
                RoleId = roleId,
                PermissionCodes = model.SelectedPermissions ?? new()
            };

            await _apiClient.AssignPermissionsToRoleAsync(roleId, request);
            TempData["Success"] = "Gán quyền cho vai trò thành công.";
            return RedirectToAction(nameof(Details), new { id = roleId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permissions to role {RoleId}", roleId);
            ModelState.AddModelError(string.Empty, "Không thể gán quyền. Vui lòng thử lại.");
            
            var permissions = await _apiClient.GetPermissionsAsync(isActive: true);
            model.AvailablePermissions = permissions;
            return View(model);
        }
    }
}

