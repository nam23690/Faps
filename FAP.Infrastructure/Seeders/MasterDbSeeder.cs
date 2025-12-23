using FAP.Common.Infrastructure.EntitiesMaster;
using FAP.Common.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FAP.Infrastructure.Seeders
{
    public class MasterDbSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly MasterDbContext _context;

        public MasterDbSeeder(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            MasterDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Đảm bảo database được tạo
            await _context.Database.MigrateAsync();

            // 1. Seed Permissions
            await SeedPermissionsAsync();

            // 2. Seed Roles
            await SeedRolesAsync();

            // 3. Gán Permissions vào Roles
            await AssignPermissionsToRolesAsync();

            // 4. Seed Users
            await SeedUsersAsync();
        }

        private async Task SeedPermissionsAsync()
        {
            var permissions = new List<AppPermission>
            {
                // User Management Permissions
                new AppPermission { Module = "User", Code = "User.View", Description = "Xem danh sách user", IsActive = true },
                new AppPermission { Module = "User", Code = "User.Create", Description = "Tạo user mới", IsActive = true },
                new AppPermission { Module = "User", Code = "User.Update", Description = "Cập nhật user", IsActive = true },
                new AppPermission { Module = "User", Code = "User.Delete", Description = "Xóa user", IsActive = true },
                new AppPermission { Module = "User", Code = "User.Import", Description = "Import user từ Excel", IsActive = true },
                new AppPermission { Module = "User", Code = "User.Export", Description = "Export user ra Excel", IsActive = true },

                // Term Management Permissions
                new AppPermission { Module = "Term", Code = "Term.View", Description = "Xem danh sách học kỳ", IsActive = true },
                new AppPermission { Module = "Term", Code = "Term.Create", Description = "Tạo học kỳ mới", IsActive = true },
                new AppPermission { Module = "Term", Code = "Term.Update", Description = "Cập nhật học kỳ", IsActive = true },
                new AppPermission { Module = "Term", Code = "Term.Delete", Description = "Xóa học kỳ", IsActive = true },

               
                new AppPermission { Module = "Permission", Code = "Permission.View", Description = "Xem danh sách permissions", IsActive = true },
                new AppPermission { Module = "Role", Code = "Role.View", Description = "Xem danh sách roles", IsActive = true },
                new AppPermission { Module = "Permission", Code = "Permission.Assign", Description = "Gán quyền cho user", IsActive = true },
                new AppPermission { Module = "Role", Code = "Role.Assign", Description = "Gán role cho user", IsActive = true },
            };

            foreach (var permission in permissions)
            {
                var existingPermission = await _context.AppPermissions
                    .FirstOrDefaultAsync(p => p.Code == permission.Code);

                if (existingPermission == null)
                {
                    _context.AppPermissions.Add(permission);
                }
                else
                {
                    // Cập nhật nếu đã tồn tại
                    existingPermission.Module = permission.Module;
                    existingPermission.Description = permission.Description;
                    existingPermission.IsActive = permission.IsActive;
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[] { "admin", "manager", "user" };

            foreach (var roleName in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                }
            }
        }

        private async Task AssignPermissionsToRolesAsync()
        {
            // Lấy tất cả permissions
            var allPermissions = await _context.AppPermissions
                .Where(p => p.IsActive)
                .ToListAsync();

            // Admin role có tất cả quyền
            var adminRole = await _roleManager.FindByNameAsync("admin");
            if (adminRole != null)
            {
                var adminClaims = await _roleManager.GetClaimsAsync(adminRole);
                var existingPermissionClaims = adminClaims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToList();

                foreach (var permission in allPermissions)
                {
                    if (!existingPermissionClaims.Contains(permission.Code))
                    {
                        await _roleManager.AddClaimAsync(adminRole, new System.Security.Claims.Claim("Permission", permission.Code));
                    }
                }
            }

            // Manager role có quyền User và Term (bao gồm cả View)
            var managerRole = await _roleManager.FindByNameAsync("manager");
            if (managerRole != null)
            {
                var managerClaims = await _roleManager.GetClaimsAsync(managerRole);
                var existingPermissionClaims = managerClaims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToList();

                var managerPermissions = allPermissions
                    .Where(p => p.Module == "User" || p.Module == "Term")
                    .ToList();

                foreach (var permission in managerPermissions)
                {
                    if (!existingPermissionClaims.Contains(permission.Code))
                    {
                        await _roleManager.AddClaimAsync(managerRole, new System.Security.Claims.Claim("Permission", permission.Code));
                    }
                }
            }

            // User role chỉ có quyền xem (View)
            var userRole = await _roleManager.FindByNameAsync("user");
            if (userRole != null)
            {
                var userClaims = await _roleManager.GetClaimsAsync(userRole);
                var existingPermissionClaims = userClaims
                    .Where(c => c.Type == "Permission")
                    .Select(c => c.Value)
                    .ToList();

                var viewPermissions = allPermissions
                    .Where(p => p.Code.EndsWith(".View")
                                && (p.Module == "User" || p.Module == "Term" || p.Module == "UserManagement"))
                    .ToList();

                foreach (var permission in viewPermissions)
                {
                    if (!existingPermissionClaims.Contains(permission.Code))
                    {
                        await _roleManager.AddClaimAsync(userRole, new System.Security.Claims.Claim("Permission", permission.Code));
                    }
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            // Tạo admin user
            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@fap.edu.vn",
                    EmailConfirmed = true,
                    FullName = "Administrator",
                    CampusId = "HN",
                    CreatedAt = DateTime.UtcNow,
                    IsLocked = 0
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    // Gán admin role
                    await _userManager.AddToRoleAsync(adminUser, "admin");

                    // Gán tất cả permissions (admin có tất cả quyền qua role)
                    var adminRole = await _roleManager.FindByNameAsync("admin");
                    if (adminRole != null)
                    {
                        var roleClaims = await _roleManager.GetClaimsAsync(adminRole);
                        var permissionClaims = roleClaims
                            .Where(c => c.Type == "Permission")
                            .ToList();

                        if (permissionClaims.Any())
                        {
                            await _userManager.AddClaimsAsync(adminUser, permissionClaims);
                        }
                    }
                }
            }

            // Tạo manager user
            var managerUser = await _userManager.FindByNameAsync("manager");
            if (managerUser == null)
            {
                managerUser = new ApplicationUser
                {
                    UserName = "manager",
                    Email = "manager@fap.edu.vn",
                    EmailConfirmed = true,
                    FullName = "Manager",
                    CampusId = "HCM",
                    CreatedAt = DateTime.UtcNow,
                    IsLocked = 0
                };

                var result = await _userManager.CreateAsync(managerUser, "Manager@123");
                if (result.Succeeded)
                {
                    // Gán manager role
                    await _userManager.AddToRoleAsync(managerUser, "manager");

                    // Gán permissions từ role
                    var managerRole = await _roleManager.FindByNameAsync("manager");
                    if (managerRole != null)
                    {
                        var roleClaims = await _roleManager.GetClaimsAsync(managerRole);
                        var permissionClaims = roleClaims
                            .Where(c => c.Type == "Permission")
                            .ToList();

                        if (permissionClaims.Any())
                        {
                            await _userManager.AddClaimsAsync(managerUser, permissionClaims);
                        }
                    }
                }
            }

            // Tạo normal user
            var normalUser = await _userManager.FindByNameAsync("user");
            if (normalUser == null)
            {
                normalUser = new ApplicationUser
                {
                    UserName = "user",
                    Email = "user@fap.edu.vn",
                    EmailConfirmed = true,
                    FullName = "Normal User",
                    CampusId = "HCM",
                    CreatedAt = DateTime.UtcNow,
                    IsLocked = 0
                };

                var result = await _userManager.CreateAsync(normalUser, "User@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(normalUser, "user");
                }
            }
        }
    }
}

