using FAP.API.Backend.Models;
using FAP.Share.Dtos;
using FAP.Common.Application.Features.User.Commands;
using FAP.Common.Application.Features.User.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FAP.API.Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly RoleManager<FAP.Common.Infrastructure.EntitiesMaster.ApplicationRole> _roleManager;

        public UserController(IMediator mediator, RoleManager<FAP.Common.Infrastructure.EntitiesMaster.ApplicationRole> roleManager)
        {
            _mediator = mediator;
            _roleManager = roleManager;
        }

        #region User Management

        /// <summary>
        /// Lấy danh sách tất cả users (không phân trang)
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetIdentityUsersQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách users (phân trang)
        /// </summary>
        [HttpGet("users/paging")]
        public async Task<IActionResult> GetUsersPaging([FromQuery] GetIdentityUsersPagingQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin user theo ID
        /// </summary>
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetIdentityUserByIdQuery { Id = id }, cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Tạo user mới
        /// </summary>
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateIdentityUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateIdentityUserCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Xóa user
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteIdentityUserCommand { Id = id }, cancellationToken);
            return NoContent();
        }

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Import([FromForm] ImportUserRequest request, CancellationToken token)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Vui lòng chọn file Excel hợp lệ.");

            var command = new ImportUserCommand { File = request.File };
            var result = await _mediator.Send(command, token);

            if (result.Errors.Any())
            {
                return BadRequest(new
                {
                    message = "Có lỗi xảy ra khi import file.",
                    errors = result.Errors,
                    result.TotalCount,
                    result.SuccessCount,
                    result.FailedCount
                });
            }

            return Ok(new
            {
                message = $"Đã import file {request.File.FileName} thành công.",
                fileName = request.File.FileName,
                fileSize = request.File.Length,
                totalCount = result.TotalCount,
                successCount = result.SuccessCount,
                failedCount = result.FailedCount,
                successUsers = result.SuccessUsers,
                failedUsers = result.FailedUsers
            });
        }

        #endregion

        #region Role Permission Management

        /// <summary>
        /// Lấy danh sách permission codes của role
        /// </summary>
        [HttpGet("roles/{roleId}/permissions")]
        public async Task<IActionResult> GetRolePermissions(string roleId, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();

            var claims = await _roleManager.GetClaimsAsync(role);
            var permissions = claims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            return Ok(permissions);
        }

        /// <summary>
        /// Gán (cập nhật) permissions cho role (thay thế danh sách hiện tại)
        /// </summary>
        public class AssignPermissionToRoleRequest
        {
            public string RoleId { get; set; } = string.Empty;
            public List<string> PermissionCodes { get; set; } = new();
        }

        [HttpPost("roles/{roleId}/permissions")]
        public async Task<IActionResult> AssignPermissionsToRole(string roleId, [FromBody] AssignPermissionToRoleRequest request, CancellationToken cancellationToken)
        {
            if (roleId != request.RoleId && !string.IsNullOrEmpty(request.RoleId))
                return BadRequest("RoleId mismatch");

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            var existingPermissionCodes = existingClaims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

            var toAdd = request.PermissionCodes.Except(existingPermissionCodes).ToList();
            var toRemove = existingPermissionCodes.Except(request.PermissionCodes).ToList();

            foreach (var code in toAdd)
            {
                await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", code));
            }

            foreach (var code in toRemove)
            {
                var claim = existingClaims.FirstOrDefault(c => c.Type == "Permission" && c.Value == code);
                if (claim != null)
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }
            }

            return NoContent();
        }

        #endregion

        #region Role Management

        /// <summary>
        /// Lấy danh sách tất cả roles
        /// </summary>
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetRolesQuery(), cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Tạo role mới
        /// </summary>
        public class CreateRoleRequest
        {
            public string Name { get; set; } = string.Empty;
        }

        /// <summary>
        /// Cập nhật role
        /// </summary>
        public class UpdateRoleRequest
        {
            public string Name { get; set; } = string.Empty;
        }

        /// <summary>
        /// Tạo role mới
        /// </summary>
        [HttpPost("roles")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Role name is required.");
            }

            var existingRole = await _roleManager.FindByNameAsync(request.Name);
            if (existingRole != null)
            {
                return BadRequest($"Role '{request.Name}' đã tồn tại.");
            }

            var role = new FAP.Common.Infrastructure.EntitiesMaster.ApplicationRole
            {
                Name = request.Name,
                NormalizedName = request.Name.ToUpperInvariant()
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var dto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                NormalizedName = role.NormalizedName,
                UserCount = 0
            };

            return Ok(dto);
        }

        /// <summary>
        /// Cập nhật role
        /// </summary>
        [HttpPost("roles/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Role name is required.");
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpperInvariant();

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        /// <summary>
        /// Gán roles cho user
        /// </summary>
        [HttpPost("users/{userId}/roles")]
        public async Task<IActionResult> AssignRoles(string userId, [FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
        {
            request.UserId = userId;
            await _mediator.Send(new AssignRoleCommand
            {
                UserId = request.UserId,
                RoleNames = request.RoleNames,
                CampusCode = request.CampusCode
            }, cancellationToken);
            return NoContent();
        }

        #endregion

        #region Permission Management

        /// <summary>
        /// Lấy danh sách permissions (có thể filter theo module và isActive)
        /// </summary>
        [HttpGet("permissions")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetPermissions([FromQuery] string? module, [FromQuery] bool? isActive, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPermissionsQuery
            {
                Module = module,
                IsActive = isActive
            }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách permissions của user
        /// </summary>
        [HttpGet("users/{userId}/permissions")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetUserPermissions(string userId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserPermissionsQuery { UserId = userId }, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gán permissions cho user (sử dụng Claim)
        /// </summary>
        [HttpPost("users/{userId}/permissions")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignPermissions(string userId, [FromBody] AssignPermissionRequest request, CancellationToken cancellationToken)
        {
            request.UserId = userId;
            await _mediator.Send(new AssignPermissionCommand
            {
                UserId = request.UserId,
                PermissionCodes = request.PermissionCodes,
                CampusCode = request.CampusCode
            }, cancellationToken);
            return NoContent();
        }

        #endregion
    }
}

