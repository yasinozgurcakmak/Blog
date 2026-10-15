using DtoLayer.Dtos.UserDtos;
using FluentValidation;

namespace Blog.ValidationRules;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Password).StrongPassword();
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Şifre tekrarı zorunludur.").Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor.");
    }
}
