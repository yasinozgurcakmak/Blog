using DtoLayer.Dtos.UserDtos;
using FluentValidation;

namespace Blog.ValidationRules;

public class RegisterValidator : AbstractValidator<UserRegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Ad alanı zorunludur.").MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");
        RuleFor(x => x.Surname).NotEmpty().WithMessage("Soyad alanı zorunludur.").MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta adresi zorunludur.").EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.").MaximumLength(256).WithMessage("E-posta adresi en fazla 256 karakter olabilir.");

        RuleFor(x => x.Password).StrongPassword();

        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Şifre tekrarı zorunludur.").Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor.");
        RuleFor(x => x.AcceptTerms).Equal(true).WithMessage("Devam etmek için kullanım koşullarını kabul etmelisiniz.");
    }
}
