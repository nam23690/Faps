using FAP.Share.Dtos;
using FAP.Shared.Http.Clients;

namespace FAP.Admin.Web.Services;

/// <summary>
/// API client implementation for User Management operations
/// </summary>
public class UserManagementApiClient : ApiClientBase, IUserManagementApiClient
{
    public UserManagementApiClient(HttpClient httpClient)
        : base(httpClient)
    {
    }

    // User Management
    public async Task<List<IdentityUserDto>> GetUsersAsync()
    {
        return await GetAsync<List<IdentityUserDto>>("/api/user/users") ?? new();
    }

    public async Task<PagedResult<IdentityUserDto>> GetUsersPagingAsync(int pageIndex = 1, int pageSize = 10)
    {
        return await GetAsync<PagedResult<IdentityUserDto>>($"/api/user/users/paging?pageIndex={pageIndex}&pageSize={pageSize}") 
            ?? new PagedResult<IdentityUserDto>();
    }

    public async Task<IdentityUserDto?> GetUserByIdAsync(string id)
    {
        return await GetAsync<IdentityUserDto>($"/api/user/users/{id}");
    }

    public async Task<IdentityUserDto> CreateUserAsync(CreateUserRequest request)
    {
        var result = await PostAsync<CreateUserRequest, IdentityUserDto>("/api/user/users", request);
        return result ?? throw new InvalidOperationException("Failed to create user");
    }

    public async Task UpdateUserAsync(string id, UpdateUserRequest request)
    {
        await PostAsync<UpdateUserRequest>($"/api/user/users/{id}", request);
    }

    public async Task DeleteUserAsync(string id)
    {
        var response = await HttpClient.DeleteAsync($"/api/user/users/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Role Management
    public async Task<List<RoleDto>> GetRolesAsync()
    {
        return await GetAsync<List<RoleDto>>("/api/user/roles") ?? new();
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleRequest request)
    {
        var result = await PostAsync<CreateRoleRequest, RoleDto>("/api/user/roles", request);
        return result ?? throw new InvalidOperationException("Failed to create role");
    }

    public async Task UpdateRoleAsync(string id, UpdateRoleRequest request)
    {
        await PostAsync<UpdateRoleRequest>($"/api/user/roles/{id}", request);
    }

    public async Task AssignRolesToUserAsync(string userId, AssignRoleRequest request)
    {
        request.UserId = userId;
        await PostAsync<AssignRoleRequest>($"/api/user/users/{userId}/roles", request);
    }

    // Permission Management
    public async Task<List<PermissionDto>> GetPermissionsAsync(string? module = null, bool? isActive = null)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(module))
            queryParams.Add($"module={Uri.EscapeDataString(module)}");
        if (isActive.HasValue)
            queryParams.Add($"isActive={isActive.Value}");

        var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        return await GetAsync<List<PermissionDto>>($"/api/user/permissions{query}") ?? new();
    }

    public async Task<List<string>> GetUserPermissionsAsync(string userId)
    {
        return await GetAsync<List<string>>($"/api/user/users/{userId}/permissions") ?? new();
    }

    public async Task AssignPermissionsToUserAsync(string userId, AssignPermissionRequest request)
    {
        request.UserId = userId;
        await PostAsync<AssignPermissionRequest>($"/api/user/users/{userId}/permissions", request);
    }

    // Role Permission Management
    public async Task<List<string>> GetRolePermissionsAsync(string roleId)
    {
        // TODO: Cần thêm API endpoint: GET /api/user/roles/{roleId}/permissions
        // Nếu endpoint chưa có (404) hoặc server lỗi, trả về danh sách rỗng thay vì ném ngoại lệ
        try
        {
            return await GetAsync<List<string>>($"/api/user/roles/{roleId}/permissions") ?? new();
        }
        catch (HttpRequestException ex) when (ex.Data.Contains("StatusCode") && ex.Data["StatusCode"] is System.Net.HttpStatusCode status && status == System.Net.HttpStatusCode.NotFound)
        {
            // Endpoint chưa tồn tại -> trả về rỗng để UI vẫn hiển thị được thông tin role
            return new List<string>();
        }
        catch
        {
            // Các lỗi khác, bubble up để controller xử lý (hoặc thay đổi theo yêu cầu)
            throw;
        }
    }

    public async Task AssignPermissionsToRoleAsync(string roleId, AssignPermissionToRoleRequest request)
    {
        request.RoleId = roleId;
        // TODO: Cần thêm API endpoint: POST /api/user/roles/{roleId}/permissions
        await PostAsync<AssignPermissionToRoleRequest>($"/api/user/roles/{roleId}/permissions", request);
    }

   
}

