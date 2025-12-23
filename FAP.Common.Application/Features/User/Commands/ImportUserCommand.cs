using FAP.Common.Application.Attributes;
using FAP.Share.Dtos;
using FAP.Common.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;


namespace FAP.Common.Application.Features.User.Commands
{
    [FapPermission("User.Import")]
    public class ImportUserCommand : IRequest<ImportUserResult>
    {
        public IFormFile File { get; set; } = null!;
        public string? DefaultPassword { get; set; } = "Password123!"; // Mật khẩu mặc định cho user import
        public string? CampusId { get; set; }
        public List<string>? DefaultRoles { get; set; }

            public class Handler : IRequestHandler<ImportUserCommand, ImportUserResult>
            {
                private readonly IExcelService _excelService;
                private readonly IMasterRepository _master;
                private readonly IMediator _mediator;

                public Handler(
                    IExcelService excelService,
                    IMasterRepository master,
                    IMediator mediator)
                {
                    _excelService = excelService;
                    _master = master;
                    _mediator = mediator;
                }

            public async Task<ImportUserResult> Handle(ImportUserCommand request, CancellationToken cancellationToken)
            {
                var result = new ImportUserResult();

                // Validate file
                if (request.File == null || request.File.Length == 0)
                {
                    result.Errors.Add("File không hợp lệ hoặc rỗng.");
                    return result;
                }

                // Validate file extension
                var allowedExtensions = new[] { ".xlsx", ".xls" };
                var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    result.Errors.Add($"Định dạng file không được hỗ trợ. Chỉ chấp nhận: {string.Join(", ", allowedExtensions)}");
                    return result;
                }

                try
                {
                    // Read users from Excel
                    using var stream = request.File.OpenReadStream();
                    var userImports = await _excelService.ReadUsersAsync(stream);

                    if (userImports == null || !userImports.Any())
                    {
                        result.Errors.Add("Không tìm thấy dữ liệu user trong file Excel.");
                        return result;
                    }

                    // Process each user
                    foreach (var userImport in userImports)
                    {
                        try
                        {
                            // Validate required fields
                            if (string.IsNullOrWhiteSpace(userImport.Login))
                            {
                                result.FailedCount++;
                                result.FailedUsers.Add(new ImportUserFailedItem
                                {
                                    Login = userImport.Login ?? "N/A",
                                    Email = userImport.Email,
                                    Fullname = userImport.Fullname,
                                    Error = "Tên đăng nhập không được để trống."
                                });
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(userImport.Email))
                            {
                                result.FailedCount++;
                                result.FailedUsers.Add(new ImportUserFailedItem
                                {
                                    Login = userImport.Login,
                                    Email = "N/A",
                                    Fullname = userImport.Fullname,
                                    Error = "Email không được để trống."
                                });
                                continue;
                            }

                            // Check if user already exists
                                var existingUser = await _master.FindByNameAsync(userImport.Login, cancellationToken);
                            if (existingUser != null)
                            {
                                result.FailedCount++;
                                result.FailedUsers.Add(new ImportUserFailedItem
                                {
                                    Login = userImport.Login,
                                    Email = userImport.Email,
                                    Fullname = userImport.Fullname,
                                    Error = $"Tên đăng nhập '{userImport.Login}' đã tồn tại."
                                });
                                continue;
                            }

                            // Check if email already exists
                                existingUser = await _master.FindByEmailAsync(userImport.Email, cancellationToken);
                            if (existingUser != null)
                            {
                                result.FailedCount++;
                                result.FailedUsers.Add(new ImportUserFailedItem
                                {
                                    Login = userImport.Login,
                                    Email = userImport.Email,
                                    Fullname = userImport.Fullname,
                                    Error = $"Email '{userImport.Email}' đã tồn tại."
                                });
                                continue;
                            }

                            // Create user using CreateIdentityUserCommand
                            var createCommand = new CreateIdentityUserCommand
                            {
                                UserName = userImport.Login,
                                Email = userImport.Email,
                                Password = request.DefaultPassword ?? "Password123!",
                                FullName = userImport.Fullname ?? string.Empty,
                                CampusId = request.CampusId ?? string.Empty,
                                RoleNames = request.DefaultRoles ?? new List<string>()
                            };

                            var createdUser = await _mediator.Send(createCommand, cancellationToken);
                            result.SuccessCount++;
                            result.SuccessUsers.Add(new ImportUserSuccessItem
                            {
                                Id = createdUser.Id,
                                Login = createdUser.UserName,
                                Email = createdUser.Email,
                                Fullname = createdUser.FullName
                            });
                        }
                        catch (Exception ex)
                        {
                            result.FailedCount++;
                            result.FailedUsers.Add(new ImportUserFailedItem
                            {
                                Login = userImport.Login ?? "N/A",
                                Email = userImport.Email ?? "N/A",
                                Fullname = userImport.Fullname ?? "N/A",
                                Error = $"Lỗi khi tạo user: {ex.Message}"
                            });
                        }
                    }

                    result.TotalCount = userImports.Count;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Lỗi khi đọc file Excel: {ex.Message}");
                }

                return result;
            }
        }
    }

    /// <summary>
    /// Kết quả import users
    /// </summary>
    public class ImportUserResult
    {
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<ImportUserSuccessItem> SuccessUsers { get; set; } = new();
        public List<ImportUserFailedItem> FailedUsers { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }

    /// <summary>
    /// User được tạo thành công
    /// </summary>
    public class ImportUserSuccessItem
    {
        public string Id { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
    }

    /// <summary>
    /// User tạo thất bại
    /// </summary>
    public class ImportUserFailedItem
    {
        public string Login { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}

