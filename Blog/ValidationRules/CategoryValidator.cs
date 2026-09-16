using EntityLayer.Concrete;
using FluentValidation;

namespace Blog.ValidationRules;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategori adı boş geçilemez.")
            .MinimumLength(2).WithMessage("Kategori adı en az 2 karakter olmalıdır.")
            .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");
    }
}
