using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using FAP.Common.Infrastructure.EntitiesMaster;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FAP.Common.Infrastructure.Repositories
{
    public class MasterRepository : IMasterRepository
    {
        private readonly MasterDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public MasterRepository(MasterDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<PermissionDto>> GetPermissionsAsync(string? module, bool? isActive, CancellationToken cancellationToken)
        {
            var query = _context.AppPermissions.AsQueryable();

            if (!string.IsNullOrEmpty(module))
            {
                query = query.Where(p => p.Module == module);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive.Value);
            }

            var perms = await query.ToListAsync(cancellationToken);
            return perms.Select(p => new PermissionDto
            {
                Id = p.Id,
                Module = p.Module,
                Code = p.Code,
                Description = p.Description,
                IsActive = p.IsActive
            }).ToList();
        }

        public async Task<List<PermissionDto>> GetActivePermissionsByCodesAsync(List<string> codes, CancellationToken cancellationToken)
        {
            var perms = await _context.AppPermissions
                .Where(p => p.IsActive && codes.Contains(p.Code))
                .ToListAsync(cancellationToken);

            return perms.Select(p => new PermissionDto
            {
                Id = p.Id,
                Module = p.Module,
                Code = p.Code,
                Description = p.Description,
                IsActive = p.IsActive
            }).ToList();
        }

        public async Task<List<PermissionDto>> GetPermissionsAsDtoAsync(string? module, bool? isActive, CancellationToken cancellationToken)
        {
            return await GetPermissionsAsync(module, isActive, cancellationToken);
        }

        public async Task UpdateUserClaimCampusCodeAsync(string userId, string claimType, string campusCode, CancellationToken cancellationToken)
        {
            var userClaims = await _context.Set<IdentityUserClaim<string>>()
                .Where(uc => uc.UserId == userId && uc.ClaimType == claimType)
                .ToListAsync(cancellationToken);

            foreach (var userClaim in userClaims)
            {
                _context.Entry(userClaim).Property("CampusCode").CurrentValue = campusCode;
            }
        }

        public async Task UpdateUserRoleCampusCodeAsync(string userId, List<string> roleIds, string campusCode, CancellationToken cancellationToken)
        {
            var userRoles = await _context.Set<IdentityUserRole<string>>()
                .Where(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId))
                .ToListAsync(cancellationToken);

            foreach (var userRole in userRoles)
            {
                _context.Entry(userRole).Property("CampusCode").CurrentValue = campusCode;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }


        // Campus management
        public async Task<List<CampusDto>> GetCampusesAsync(CancellationToken cancellationToken)
        {
            var campuses = await _context.Campuses.OrderBy(c => c.Name).ToListAsync(cancellationToken);
            return campuses.Select(c => new CampusDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                Address = c.Address,
                Phone = c.Phone,
                Email = c.Email,
                IsActive = c.IsActive,
                ConnectionString = c.ConnectionString
            }).ToList();
        }

        public async Task<CampusDto?> GetCampusByIdAsync(string id, CancellationToken cancellationToken)
        {
            var c = await _context.Campuses.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
            if (c == null) return null;
            return new CampusDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                Address = c.Address,
                Phone = c.Phone,
                Email = c.Email,
                IsActive = c.IsActive,
                ConnectionString = c.ConnectionString
            };
        }

        public async Task<CampusDto> CreateCampusAsync(CampusDto campus, CancellationToken cancellationToken)
        {
            var entity = new Campus
            {
                Code = campus.Code,
                Name = campus.Name,
                Description = campus.Description,
                Address = campus.Address,
                Phone = campus.Phone,
                Email = campus.Email,
                IsActive = campus.IsActive,
                ConnectionString = campus.ConnectionString
            };
            _context.Campuses.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            campus.Id = entity.Id;
            return campus;
        }

        public async Task UpdateCampusAsync(CampusDto campus, CancellationToken cancellationToken)
        {
            var existing = await _context.Campuses.FindAsync(new object[] { campus.Id }, cancellationToken: cancellationToken);
            if (existing == null) return;
            existing.Code = campus.Code;
            existing.Name = campus.Name;
            existing.Description = campus.Description;
            existing.Address = campus.Address;
            existing.Phone = campus.Phone;
            existing.Email = campus.Email;
            existing.IsActive = campus.IsActive;
            existing.ConnectionString = campus.ConnectionString;
            _context.Campuses.Update(existing);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCampusAsync(string id, CancellationToken cancellationToken)
        {
            var campus = await _context.Campuses.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
            if (campus != null)
            {
                _context.Campuses.Remove(campus);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        // User & Role management implementations
        public async Task<List<IdentityUserDto>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _context.Set<ApplicationUser>().ToListAsync(cancellationToken);
            var result = new List<IdentityUserDto>();
            foreach (var u in users)
            {
                var roles = await GetUserRolesAsync(u.Id, cancellationToken);
                var claims = await GetUserClaimsAsync(u.Id, cancellationToken);
                result.Add(new IdentityUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    FullName = u.FullName ?? string.Empty,
                    CampusId = u.CampusId ?? string.Empty,
                    EmailConfirmed = u.EmailConfirmed,
                    IsLocked = u.IsLocked == 1,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    CreatedAt = u.CreatedAt,
                    Roles = roles,
                    Permissions = claims.Select(c => c.Value).ToList()
                });
            }
            return result;
        }

        public async Task<(List<IdentityUserDto> Users, int Total)> GetUsersPagingAsync(int pageIndex, int pageSize, string? keyword, CancellationToken cancellationToken)
        {
            var query = _context.Set<ApplicationUser>().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.Contains(keyword)) ||
                    (u.Email != null && u.Email.Contains(keyword)) ||
                    (u.FullName != null && u.FullName.Contains(keyword)) ||
                    (u.CampusId != null && u.CampusId.Contains(keyword)));
            }

            var total = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderBy(u => u.UserName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new List<IdentityUserDto>();
            foreach (var u in users)
            {
                var roles = await GetUserRolesAsync(u.Id, cancellationToken);
                var claims = await GetUserClaimsAsync(u.Id, cancellationToken);
                result.Add(new IdentityUserDto
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    FullName = u.FullName ?? string.Empty,
                    CampusId = u.CampusId ?? string.Empty,
                    EmailConfirmed = u.EmailConfirmed,
                    IsLocked = u.IsLocked == 1,
                    ProfilePictureUrl = u.ProfilePictureUrl,
                    CreatedAt = u.CreatedAt,
                    Roles = roles,
                    Permissions = claims.Select(c => c.Value).ToList()
                });
            }

            return (result, total);
        }

        public async Task<IdentityUserDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken)
        {
            var u = await _context.Set<ApplicationUser>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
            if (u == null) return null;
            var roles = await GetUserRolesAsync(u.Id, cancellationToken);
            var claims = await GetUserClaimsAsync(u.Id, cancellationToken);
            return new IdentityUserDto
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName ?? string.Empty,
                CampusId = u.CampusId ?? string.Empty,
                EmailConfirmed = u.EmailConfirmed,
                IsLocked = u.IsLocked == 1,
                ProfilePictureUrl = u.ProfilePictureUrl,
                CreatedAt = u.CreatedAt,
                Roles = roles,
                Permissions = claims.Select(c => c.Value).ToList()
            };
        }

        public async Task<List<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
        {
            var roleIds = await _context.Set<IdentityUserRole<string>>()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            var roles = await _context.Set<ApplicationRole>()
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name!)
                .ToListAsync(cancellationToken);

            return roles;
        }

        public async Task<List<System.Security.Claims.Claim>> GetUserClaimsAsync(string userId, CancellationToken cancellationToken)
        {
            var userClaims = await _context.Set<IdentityUserClaim<string>>()
                .Where(uc => uc.UserId == userId)
                .ToListAsync(cancellationToken);

            return userClaims.Select(uc => new System.Security.Claims.Claim(uc.ClaimType ?? string.Empty, uc.ClaimValue ?? string.Empty)).ToList();
        }

        public async Task AddClaimsToUserAsync(string userId, IEnumerable<System.Security.Claims.Claim> claims, CancellationToken cancellationToken)
        {
            foreach (var c in claims)
            {
                var uc = new IdentityUserClaim<string>
                {
                    UserId = userId,
                    ClaimType = c.Type,
                    ClaimValue = c.Value
                };
                _context.Set<IdentityUserClaim<string>>().Add(uc);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveClaimsFromUserAsync(string userId, IEnumerable<System.Security.Claims.Claim> claims, CancellationToken cancellationToken)
        {
            var existing = await _context.Set<IdentityUserClaim<string>>()
                .Where(uc => uc.UserId == userId && claims.Select(c => c.Type).Contains(uc.ClaimType) && claims.Select(c => c.Value).Contains(uc.ClaimValue))
                .ToListAsync(cancellationToken);

            if (existing.Any())
            {
                _context.Set<IdentityUserClaim<string>>().RemoveRange(existing);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task AddUserToRolesAsync(string userId, IEnumerable<string> roleNames, CancellationToken cancellationToken)
        {
            var roles = await _context.Set<ApplicationRole>().Where(r => r.Name != null && roleNames.Contains(r.Name)).ToListAsync(cancellationToken);
            foreach (var role in roles)
            {
                var ur = new IdentityUserRole<string> { UserId = userId, RoleId = role.Id };
                _context.Set<IdentityUserRole<string>>().Add(ur);
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveUserFromRolesAsync(string userId, IEnumerable<string> roleNames, CancellationToken cancellationToken)
        {
            var roleIds = await _context.Set<ApplicationRole>().Where(r => r.Name != null && roleNames.Contains(r.Name)).Select(r => r.Id).ToListAsync(cancellationToken);
            var existing = await _context.Set<IdentityUserRole<string>>().Where(ur => ur.UserId == userId && roleIds.Contains(ur.RoleId)).ToListAsync(cancellationToken);
            if (existing.Any())
            {
                _context.Set<IdentityUserRole<string>>().RemoveRange(existing);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<RoleDto>> GetRolesAsync(CancellationToken cancellationToken)
        {
            var roles = await _context.Set<ApplicationRole>().ToListAsync(cancellationToken);
            var result = new List<RoleDto>();
            foreach (var r in roles)
            {
                var count = await _context.Set<IdentityUserRole<string>>().CountAsync(ur => ur.RoleId == r.Id, cancellationToken);
                result.Add(new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name ?? string.Empty,
                    NormalizedName = r.NormalizedName,
                    UserCount = count
                });
            }
            return result;
        }

        public async Task<int> GetUserCountInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var role = await _context.Set<ApplicationRole>().FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
            if (role == null) return 0;

            var count = await _context.Set<IdentityUserRole<string>>().CountAsync(ur => ur.RoleId == role.Id, cancellationToken);
            return count;
        }

        public async Task<RoleDto?> FindRoleByNameAsync(string roleName, CancellationToken cancellationToken)
        {
            var r = await _context.Set<ApplicationRole>().FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
            if (r == null) return null;
            return new RoleDto
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                NormalizedName = r.NormalizedName,
                UserCount = await _context.Set<IdentityUserRole<string>>().CountAsync(ur => ur.RoleId == r.Id, cancellationToken)
            };
        }

        public async Task<RoleDto?> GetRoleByIdAsync(string id, CancellationToken cancellationToken)
        {
            var r = await _context.Set<ApplicationRole>().FindAsync(new object[] { id }, cancellationToken: cancellationToken);
            if (r == null) return null;
            return new RoleDto
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty,
                NormalizedName = r.NormalizedName,
                UserCount = await _context.Set<IdentityUserRole<string>>().CountAsync(ur => ur.RoleId == r.Id, cancellationToken)
            };
        }

        public async Task CreateRoleAsync(RoleDto roleDto, CancellationToken cancellationToken)
        {
            var role = new ApplicationRole
            {
                Name = roleDto.Name,
                NormalizedName = roleDto.NormalizedName
            };
            _context.Set<ApplicationRole>().Add(role);
            await _context.SaveChangesAsync(cancellationToken);
            roleDto.Id = role.Id;
        }

        public async Task UpdateRoleAsync(RoleDto roleDto, CancellationToken cancellationToken)
        {
            var existing = await _context.Set<ApplicationRole>().FindAsync(new object[] { roleDto.Id }, cancellationToken: cancellationToken);
            if (existing == null) return;
            existing.Name = roleDto.Name;
            existing.NormalizedName = roleDto.NormalizedName;
            _context.Set<ApplicationRole>().Update(existing);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<System.Security.Claims.Claim>> GetRoleClaimsAsync(string roleId, CancellationToken cancellationToken)
        {
            var roleClaims = await _context.Set<IdentityRoleClaim<string>>().Where(rc => rc.RoleId == roleId).ToListAsync(cancellationToken);
            return roleClaims.Select(rc => new System.Security.Claims.Claim(rc.ClaimType ?? string.Empty, rc.ClaimValue ?? string.Empty)).ToList();
        }

        public async Task AddClaimToRoleAsync(string roleId, System.Security.Claims.Claim claim, CancellationToken cancellationToken)
        {
            var rc = new IdentityRoleClaim<string>
            {
                RoleId = roleId,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
            _context.Set<IdentityRoleClaim<string>>().Add(rc);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveClaimFromRoleAsync(string roleId, System.Security.Claims.Claim claim, CancellationToken cancellationToken)
        {
            var existing = await _context.Set<IdentityRoleClaim<string>>().Where(rc => rc.RoleId == roleId && rc.ClaimType == claim.Type && rc.ClaimValue == claim.Value).ToListAsync(cancellationToken);
            if (existing.Any())
            {
                _context.Set<IdentityRoleClaim<string>>().RemoveRange(existing);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        // Identity operations using UserManager/RoleManager (work with DTOs at application boundary)
        public async Task<IdentityOperationResultDto> CreateUserAsync(IdentityUserDto userDto, string password, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                FullName = userDto.FullName,
                CampusId = userDto.CampusId,
                ProfilePictureUrl = userDto.ProfilePictureUrl ?? string.Empty,
                CreatedAt = userDto.CreatedAt
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded && userDto.Roles?.Any() == true)
            {
                await _userManager.AddToRolesAsync(user, userDto.Roles);
            }

            // propagate created id back to DTO
            userDto.Id = user.Id;

            return new IdentityOperationResultDto
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors?.Select(e => e.Description ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>()
            };
        }

        public async Task<IdentityOperationResultDto> UpdateUserAsync(IdentityUserDto userDto, CancellationToken cancellationToken)
        {
            var existing = await _userManager.FindByIdAsync(userDto.Id);
            if (existing == null) return new IdentityOperationResultDto { Succeeded = false, Errors = new List<string> { "User not found" } };
            existing.UserName = userDto.UserName;
            existing.Email = userDto.Email;
            existing.FullName = userDto.FullName;
            existing.CampusId = userDto.CampusId;
            existing.ProfilePictureUrl = userDto.ProfilePictureUrl ?? string.Empty;
            var result = await _userManager.UpdateAsync(existing);
            return new IdentityOperationResultDto
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors?.Select(e => e.Description ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>()
            };
        }

        public async Task<IdentityOperationResultDto> DeleteUserAsync(IdentityUserDto userDto, CancellationToken cancellationToken)
        {
            var existing = await _userManager.FindByIdAsync(userDto.Id);
            if (existing == null) return new IdentityOperationResultDto { Succeeded = false, Errors = new List<string> { "User not found" } };
            var result = await _userManager.DeleteAsync(existing);
            return new IdentityOperationResultDto
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors?.Select(e => e.Description ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>()
            };
        }

        public async Task<bool> CheckPasswordAsync(IdentityUserDto userDto, string password)
        {
            var existing = await _userManager.FindByIdAsync(userDto.Id);
            if (existing == null) return false;
            return await _userManager.CheckPasswordAsync(existing, password);
        }

        public async Task<IdentityOperationResultDto> AddUserToRolesByNamesAsync(IdentityUserDto userDto, IEnumerable<string> roleNames, CancellationToken cancellationToken)
        {
            var existing = await _userManager.FindByIdAsync(userDto.Id);
            if (existing == null) return new IdentityOperationResultDto { Succeeded = false, Errors = new List<string> { "User not found" } };
            var result = await _userManager.AddToRolesAsync(existing, roleNames);
            return new IdentityOperationResultDto
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors?.Select(e => e.Description ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>()
            };
        }

        public async Task<IdentityOperationResultDto> RemoveUserFromRolesByNamesAsync(IdentityUserDto userDto, IEnumerable<string> roleNames, CancellationToken cancellationToken)
        {
            var existing = await _userManager.FindByIdAsync(userDto.Id);
            if (existing == null) return new IdentityOperationResultDto { Succeeded = false, Errors = new List<string> { "User not found" } };
            var result = await _userManager.RemoveFromRolesAsync(existing, roleNames);
            return new IdentityOperationResultDto
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors?.Select(e => e.Description ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToList() ?? new List<string>()
            };
        }

        public async Task<IdentityUserDto?> FindByNameAsync(string userName, CancellationToken cancellationToken)
        {
            var u = await _userManager.FindByNameAsync(userName);
            if (u == null) return null;
            var roles = await GetUserRolesAsync(u.Id, cancellationToken);
            var claims = await GetUserClaimsAsync(u.Id, cancellationToken);
            return new IdentityUserDto
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName ?? string.Empty,
                CampusId = u.CampusId ?? string.Empty,
                EmailConfirmed = u.EmailConfirmed,
                IsLocked = u.IsLocked == 1,
                ProfilePictureUrl = u.ProfilePictureUrl,
                CreatedAt = u.CreatedAt,
                Roles = roles,
                Permissions = claims.Select(c => c.Value).ToList()
            };
        }

        public async Task<IdentityUserDto?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var u = await _userManager.FindByEmailAsync(email);
            if (u == null) return null;
            var roles = await GetUserRolesAsync(u.Id, cancellationToken);
            var claims = await GetUserClaimsAsync(u.Id, cancellationToken);
            return new IdentityUserDto
            {
                Id = u.Id,
                UserName = u.UserName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName ?? string.Empty,
                CampusId = u.CampusId ?? string.Empty,
                EmailConfirmed = u.EmailConfirmed,
                IsLocked = u.IsLocked == 1,
                ProfilePictureUrl = u.ProfilePictureUrl,
                CreatedAt = u.CreatedAt,
                Roles = roles,
                Permissions = claims.Select(c => c.Value).ToList()
            };
        }
    }
}

