using Blog.Helper;
using DtoLayer.Dtos.BlogDtos;
using FluentValidation;

namespace Blog.ValidationRules;

public class UpdateBlogValidator : AbstractValidator<UpdateBlogDto>
{
    public UpdateBlogValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Geçersiz blog kaydı.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Blog başlığı zorunludur.").MinimumLength(3).WithMessage("Başlık en az 3 karakter olmalıdır.").MaximumLength(200).WithMessage("Başlık en fazla 200 karakter olabilir.");
        RuleFor(x => x.Summary).NotEmpty().WithMessage("Blog özeti zorunludur.").MaximumLength(500).WithMessage("Özet en fazla 500 karakter olabilir.");
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Lütfen geçerli bir kategori seçiniz.");
        RuleFor(x => x.Content).NotEmpty().WithMessage("Blog içeriği zorunludur.");
        RuleFor(x => x.NewThumbnailFile).Must(file => file is null || ImageHelper.IsValidImage(file, out _)).WithMessage("Geçersiz yeni küçük resim dosyası. (Yalnızca .jpg, .jpeg, .png, .webp ve en fazla 5MB kabul edilir.)");
        RuleFor(x => x).Must(x => !string.IsNullOrWhiteSpace(x.ExistingThumbnail) || x.NewThumbnailFile is not null).WithMessage("Lütfen bir küçük resim (Thumbnail) belirleyiniz.");
    }
}
