using FAP.Share.Dtos;
using FAP.Shared.Http.Abstractions;

namespace FAP.Admin.Web.Services;

/// <summary>
/// API client interface for User Management operations
/// </summary>
public interface IUserManagementApiClient : IApiClient
{
    // User Management
    Task<List<IdentityUserDto>> GetUsersAsync();
    Task<PagedResult<IdentityUserDto>> GetUsersPagingAsync(int pageIndex = 1, int pageSize = 10);
    Task<IdentityUserDto?> GetUserByIdAsync(string id);
    Task<IdentityUserDto> CreateUserAsync(CreateUserRequest request);
    Task UpdateUserAsync(string id, UpdateUserRequest request);
    Task DeleteUserAsync(string id);

    // Role Management
    Task<List<RoleDto>> GetRolesAsync();
    Task<RoleDto> CreateRoleAsync(CreateRoleRequest request);
    Task UpdateRoleAsync(string id, UpdateRoleRequest request);
    Task AssignRolesToUserAsync(string userId, AssignRoleRequest request);

    // Permission Management
    Task<List<PermissionDto>> GetPermissionsAsync(string? module = null, bool? isActive = null);
    Task<List<string>> GetUserPermissionsAsync(string userId);
    Task AssignPermissionsToUserAsync(string userId, AssignPermissionRequest request);
    
    // Role Permission Management
    Task<List<string>> GetRolePermissionsAsync(string roleId);
    Task AssignPermissionsToRoleAsync(string roleId, AssignPermissionToRoleRequest request);
}

/// <summary>
/// Request model for creating a user
/// </summary>
public class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string CampusId { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public List<string> RoleNames { get; set; } = new();
    public string? CampusCode { get; set; }
}

/// <summary>
/// Request model for updating a user
/// </summary>
public class UpdateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string CampusId { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public bool IsLocked { get; set; }
}


/// <summary>
/// Request model for assigning permissions to role
/// </summary>
public class AssignPermissionToRoleRequest
{
    public string RoleId { get; set; } = string.Empty;
    public List<string> PermissionCodes { get; set; } = new();
}

/// <summary>
/// Request model for creating a role
/// </summary>
public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Request model for updating a role
/// </summary>
public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
}

