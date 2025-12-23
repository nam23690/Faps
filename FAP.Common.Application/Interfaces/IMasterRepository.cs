using FAP.Share.Dtos;

namespace FAP.Common.Application.Interfaces
{
    public interface IMasterRepository
    {
        Task<List<PermissionDto>> GetPermissionsAsync(string? module, bool? isActive, CancellationToken cancellationToken);
        Task<List<PermissionDto>> GetActivePermissionsByCodesAsync(List<string> codes, CancellationToken cancellationToken);
        Task<List<PermissionDto>> GetPermissionsAsDtoAsync(string? module, bool? isActive, CancellationToken cancellationToken);
        Task UpdateUserClaimCampusCodeAsync(string userId, string claimType, string campusCode, CancellationToken cancellationToken);
        Task UpdateUserRoleCampusCodeAsync(string userId, List<string> roleIds, string campusCode, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        // Campus management
        Task<List<CampusDto>> GetCampusesAsync(CancellationToken cancellationToken);
        Task<CampusDto?> GetCampusByIdAsync(string id, CancellationToken cancellationToken);
        Task<CampusDto> CreateCampusAsync(CampusDto campus, CancellationToken cancellationToken);
        Task UpdateCampusAsync(CampusDto campus, CancellationToken cancellationToken);
        Task DeleteCampusAsync(string id, CancellationToken cancellationToken);
        // User & Role management (Identity helpers so Application layer doesn't depend on UserManager/RoleManager)
        Task<List<IdentityUserDto>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<(List<IdentityUserDto> Users, int Total)> GetUsersPagingAsync(int pageIndex, int pageSize, string? keyword, CancellationToken cancellationToken);
        Task<IdentityUserDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken);
        Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken);
        Task<List<System.Security.Claims.Claim>> GetUserClaimsAsync(string userId, CancellationToken cancellationToken);
        Task AddClaimsToUserAsync(string userId, IEnumerable<System.Security.Claims.Claim> claims, CancellationToken cancellationToken);
        Task RemoveClaimsFromUserAsync(string userId, IEnumerable<System.Security.Claims.Claim> claims, CancellationToken cancellationToken);
        Task AddUserToRolesAsync(string userId, IEnumerable<string> roleNames, CancellationToken cancellationToken);
        Task RemoveUserFromRolesAsync(string userId, IEnumerable<string> roleNames, CancellationToken cancellationToken);

        Task<List<RoleDto>> GetRolesAsync(CancellationToken cancellationToken);
        Task<int> GetUserCountInRoleAsync(string roleName, CancellationToken cancellationToken);
        Task<RoleDto?> FindRoleByNameAsync(string roleName, CancellationToken cancellationToken);
        Task<RoleDto?> GetRoleByIdAsync(string id, CancellationToken cancellationToken);
        Task CreateRoleAsync(RoleDto role, CancellationToken cancellationToken);
        Task UpdateRoleAsync(RoleDto role, CancellationToken cancellationToken);
        Task<List<System.Security.Claims.Claim>> GetRoleClaimsAsync(string roleId, CancellationToken cancellationToken);
        Task AddClaimToRoleAsync(string roleId, System.Security.Claims.Claim claim, CancellationToken cancellationToken);
        Task RemoveClaimFromRoleAsync(string roleId, System.Security.Claims.Claim claim, CancellationToken cancellationToken);
        // User management with identity operations (may delegate to UserManager internally)
        Task<IdentityOperationResultDto> CreateUserAsync(IdentityUserDto user, string password, CancellationToken cancellationToken);
        Task<IdentityOperationResultDto> UpdateUserAsync(IdentityUserDto user, CancellationToken cancellationToken);
        Task<IdentityOperationResultDto> DeleteUserAsync(IdentityUserDto user, CancellationToken cancellationToken);
        Task<bool> CheckPasswordAsync(IdentityUserDto user, string password);
        Task<IdentityOperationResultDto> AddUserToRolesByNamesAsync(IdentityUserDto user, IEnumerable<string> roleNames, CancellationToken cancellationToken);
        Task<IdentityOperationResultDto> RemoveUserFromRolesByNamesAsync(IdentityUserDto user, IEnumerable<string> roleNames, CancellationToken cancellationToken);
        Task<IdentityUserDto?> FindByNameAsync(string userName, CancellationToken cancellationToken);
        Task<IdentityUserDto?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    }
}

