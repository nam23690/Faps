using FluentValidation;

namespace FAP.Common.Application.Features.User.Commands
{
    public class CreateIdentityUserCommandValidator : AbstractValidator<CreateIdentityUserCommand>
    {
        public CreateIdentityUserCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
                .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống");

            RuleFor(x => x.CampusId)
                .NotEmpty().WithMessage("Campus ID không được để trống");
        }
    }
}

