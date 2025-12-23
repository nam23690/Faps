using FAP.Admin.Web.Services;
using FAP.Share.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FAP.Admin.Web.Controllers;

/// <summary>
/// Controller for User Management operations
/// </summary>
[Route("usermanagement")]
public class UserManagementController : Controller
{
    private readonly ILogger<UserManagementController> _logger;
    private readonly IUserManagementApiClient _apiClient;

    public UserManagementController(
        ILogger<UserManagementController> logger,
        IUserManagementApiClient apiClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    #region User Management

    /// <summary>
    /// Danh sách users
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 1)
    {
        try
        {
            var result = await _apiClient.GetUsersPagingAsync(page, pageSize);
            return View(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
            TempData["Error"] = "Không thể tải danh sách người dùng.";
            return View(new PagedResult<IdentityUserDto>());
        }
    }

    /// <summary>
    /// Chi tiết user
    /// </summary>
    [HttpGet("details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var user = await _apiClient.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            // Load roles and permissions
            var roles = await _apiClient.GetRolesAsync();
            ViewBag.Roles = roles;
            ViewBag.Permissions = await _apiClient.GetPermissionsAsync();

            // Lấy permissions theo từng role để hiển thị trên trang chi tiết user
            var rolePermissionsMap = new Dictionary<string, List<string>>();
            foreach (var roleName in user.Roles)
            {
                var roleDto = roles.FirstOrDefault(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
                if (roleDto != null)
                {
                    try
                    {
                        var perms = await _apiClient.GetRolePermissionsAsync(roleDto.Id);
                        rolePermissionsMap[roleName] = perms ?? new List<string>();
                    }
                    catch
                    {
                        rolePermissionsMap[roleName] = new List<string>();
                    }
                }
                else
                {
                    rolePermissionsMap[roleName] = new List<string>();
                }
            }

            ViewBag.RolePermissionsMap = rolePermissionsMap;
            // Map role name -> role id for view usage (avoid needing strong type in Razor)
            var roleIdMap = roles.ToDictionary(r => r.Name ?? string.Empty, r => r.Id ?? string.Empty, StringComparer.OrdinalIgnoreCase);
            ViewBag.RoleIdMap = roleIdMap;

            return View(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user details for {UserId}", id);
            TempData["Error"] = "Không thể tải thông tin người dùng.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Form tạo user mới
    /// </summary>
    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = await _apiClient.GetRolesAsync();
        return View(new CreateUserViewModel());
    }

    /// <summary>
    /// Xử lý tạo user mới
    /// </summary>
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _apiClient.GetRolesAsync();
            return View(model);
        }

        try
        {
            var request = new CreateUserRequest
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                FullName = model.FullName,
                CampusId = model.CampusId,
                ProfilePictureUrl = model.ProfilePictureUrl,
                RoleNames = model.SelectedRoles ?? new(),
                CampusCode = model.CampusCode
            };

            await _apiClient.CreateUserAsync(request);
            TempData["Success"] = "Tạo người dùng thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            ModelState.AddModelError(string.Empty, "Không thể tạo người dùng. Vui lòng thử lại.");
            ViewBag.Roles = await _apiClient.GetRolesAsync();
            return View(model);
        }
    }

    /// <summary>
    /// Form chỉnh sửa user
    /// </summary>
    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        try
        {
            var user = await _apiClient.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                CampusId = user.CampusId,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsLocked = user.IsLocked
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit {UserId}", id);
            TempData["Error"] = "Không thể tải thông tin người dùng.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Xử lý cập nhật user
    /// </summary>
    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditUserViewModel model)
    {
        if (id != model.Id)
        {
            TempData["Error"] = "ID không khớp.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new UpdateUserRequest
            {
                Email = model.Email,
                FullName = model.FullName,
                CampusId = model.CampusId,
                ProfilePictureUrl = model.ProfilePictureUrl,
                IsLocked = model.IsLocked
            };

            await _apiClient.UpdateUserAsync(id, request);
            TempData["Success"] = "Cập nhật người dùng thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            ModelState.AddModelError(string.Empty, "Không thể cập nhật người dùng. Vui lòng thử lại.");
            return View(model);
        }
    }

    /// <summary>
    /// Xóa user
    /// </summary>
    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _apiClient.DeleteUserAsync(id);
            TempData["Success"] = "Xóa người dùng thành công.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            TempData["Error"] = "Không thể xóa người dùng. Vui lòng thử lại.";
        }

        return RedirectToAction(nameof(Index));
    }

    #endregion

    #region Role Assignment

    /// <summary>
    /// Form gán roles cho user
    /// </summary>
    [HttpGet("assign-roles/{userId}")]
    public async Task<IActionResult> AssignRoles(string userId)
    {
        try
        {
            var user = await _apiClient.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _apiClient.GetRolesAsync();
            var model = new AssignRolesViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                FullName = user.FullName,
                CurrentRoles = user.Roles,
                AvailableRoles = roles.Select(r => r.Name).ToList()
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading assign roles for user {UserId}", userId);
            TempData["Error"] = "Không thể tải thông tin.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Xử lý gán roles cho user
    /// </summary>
    [HttpPost("assign-roles/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRoles(string userId, AssignRolesViewModel model)
    {
        if (userId != model.UserId)
        {
            TempData["Error"] = "ID không khớp.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var request = new AssignRoleRequest
            {
                UserId = userId,
                RoleNames = model.SelectedRoles ?? new(),
                CampusCode = model.CampusCode
            };

            await _apiClient.AssignRolesToUserAsync(userId, request);
            TempData["Success"] = "Gán vai trò thành công.";
            return RedirectToAction(nameof(Details), new { id = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning roles to user {UserId}", userId);
            ModelState.AddModelError(string.Empty, "Không thể gán vai trò. Vui lòng thử lại.");
            
            var roles = await _apiClient.GetRolesAsync();
            model.AvailableRoles = roles.Select(r => r.Name).ToList();
            return View(model);
        }
    }

    #endregion

    #region Permission Assignment

    /// <summary>
    /// Form gán permissions cho user
    /// </summary>
    [HttpGet("assign-permissions/{userId}")]
    public async Task<IActionResult> AssignPermissions(string userId)
    {
        try
        {
            var user = await _apiClient.GetUserByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction(nameof(Index));
            }

            var permissions = await _apiClient.GetPermissionsAsync(isActive: true);
            var userPermissions = await _apiClient.GetUserPermissionsAsync(userId);

            var model = new AssignPermissionsViewModel
            {
                UserId = userId,
                UserName = user.UserName,
                FullName = user.FullName,
                CurrentPermissions = userPermissions,
                AvailablePermissions = permissions
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading assign permissions for user {UserId}", userId);
            TempData["Error"] = "Không thể tải thông tin.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Xử lý gán permissions cho user
    /// </summary>
    [HttpPost("assign-permissions/{userId}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignPermissions(string userId, AssignPermissionsViewModel model)
    {
        if (userId != model.UserId)
        {
            TempData["Error"] = "ID không khớp.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var request = new AssignPermissionRequest
            {
                UserId = userId,
                PermissionCodes = model.SelectedPermissions ?? new(),
                CampusCode = model.CampusCode
            };

            await _apiClient.AssignPermissionsToUserAsync(userId, request);
            TempData["Success"] = "Gán quyền thành công.";
            return RedirectToAction(nameof(Details), new { id = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permissions to user {UserId}", userId);
            ModelState.AddModelError(string.Empty, "Không thể gán quyền. Vui lòng thử lại.");
            
            var permissions = await _apiClient.GetPermissionsAsync(isActive: true);
            model.AvailablePermissions = permissions;
            return View(model);
        }
    }

    #endregion
}

