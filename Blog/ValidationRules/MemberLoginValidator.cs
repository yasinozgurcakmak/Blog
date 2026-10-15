using DtoLayer.Dtos.UserDtos;
using FluentValidation;

namespace Blog.ValidationRules;

public class MemberLoginValidator : AbstractValidator<MemberLoginDto>
{
    public MemberLoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta adresi zorunludur.").EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre zorunludur.");
    }
}
