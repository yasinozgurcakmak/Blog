using DtoLayer.Dtos.UserDtos;
using FluentValidation;

namespace Blog.ValidationRules;

public class MemberLoginValidator : AbstractValidator<MemberLoginDto>
{
    public MemberLoginValidator()
    {
        RuleFor(x => x.UserNameOrEmail).NotEmpty().WithMessage("Kullanıcı adı veya e-posta adresi zorunludur.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre zorunludur.");
    }
}
