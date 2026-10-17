using FluentValidation;

namespace Blog.ValidationRules;

public static class PasswordRules
{
    public const int MinLength = 8;

  
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> rule) => rule
        .NotEmpty().WithMessage("Şifre zorunludur.")
        .MinimumLength(MinLength).WithMessage($"Şifre en az {MinLength} karakter olmalıdır.")
        .Matches("[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
        .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
        .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
        .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir.");
}
