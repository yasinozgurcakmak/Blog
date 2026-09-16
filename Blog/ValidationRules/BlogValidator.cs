using EntityLayer.Concrete;
using FluentValidation;

namespace Blog.ValidationRules;

public class BlogValidator : AbstractValidator<EntityLayer.Concrete.Blog>
{
    public BlogValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Blog başlığı boş geçilemez.").MaximumLength(250).WithMessage("Blog başlığı en fazla 250 karakter olabilir.");
        RuleFor(x => x.Summary).NotEmpty().WithMessage("Blog özeti boş geçilemez.").MaximumLength(500).WithMessage("Blog özeti en fazla 500 karakter olabilir.");
        RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Lütfen geçerli bir kategori seçiniz.");
        RuleFor(x => x.Content).NotEmpty().WithMessage("Blog içeriği boş geçilemez.");
    }
}
