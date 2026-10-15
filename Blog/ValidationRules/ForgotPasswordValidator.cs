using DtoLayer.Dtos.UserDtos;
using FluentValidation;

namespace Blog.ValidationRules;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta adresi zorunludur.").EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
    }
}
