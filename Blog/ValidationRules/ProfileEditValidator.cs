using DtoLayer.Dtos.UserDtos;
using FluentValidation;

namespace Blog.ValidationRules;

public class ProfileEditValidator : AbstractValidator<ProfileEditDto>
{
    public const int MaxUrlLength = 200;
    public const int MaxHeadlineLength = 100;
    public const int MaxBioLength = 300;

    public ProfileEditValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Ad alanı zorunludur.").MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");
        RuleFor(x => x.Surname).NotEmpty().WithMessage("Soyad alanı zorunludur.").MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");
        RuleFor(x => x.Headline).MaximumLength(MaxHeadlineLength).WithMessage($"Rol / ünvan en fazla {MaxHeadlineLength} karakter olabilir.");
        RuleFor(x => x.Bio).MaximumLength(MaxBioLength).WithMessage($"Biyografi en fazla {MaxBioLength} karakter olabilir.");

        LinkRule(x => x.GithubUrl, "GitHub", "github.com");
        LinkRule(x => x.InstagramUrl, "Instagram", "instagram.com");
        LinkRule(x => x.XUrl, "X (Twitter)", "x.com", "twitter.com");
        LinkRule(x => x.FacebookUrl, "Facebook", "facebook.com", "fb.com");
        LinkRule(x => x.WebsiteUrl, "Web sitesi");
    }

    private void LinkRule(System.Linq.Expressions.Expression<Func<ProfileEditDto, string?>> field, string label, params string[] allowedHosts)
    {
        var getValue = field.Compile();
        RuleFor(field)
            .MaximumLength(MaxUrlLength).WithMessage($"{label} linki en fazla {MaxUrlLength} karakter olabilir.")
            .Must(url => IsValidLink(url, allowedHosts))
            .WithMessage(allowedHosts.Length > 0
                ? $"Geçerli bir {label} linki giriniz (örn. https://{allowedHosts[0]}/kullaniciadi)."
                : $"Geçerli bir {label} linki giriniz (örn. https://ornek.com).")
            .When(x => !string.IsNullOrWhiteSpace(getValue(x)));
    }

    private static bool IsValidLink(string? url, string[] allowedHosts)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
        if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) return false;
        if (allowedHosts.Length == 0) return uri.Host.Contains('.');

        var host = uri.Host.StartsWith("www.") ? uri.Host[4..] : uri.Host;
        return allowedHosts.Contains(host, StringComparer.OrdinalIgnoreCase) && uri.AbsolutePath.Trim('/').Length > 0;
    }
}
