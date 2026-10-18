using DtoLayer.Dtos.CommentDtos;
using FluentValidation;

namespace Blog.ValidationRules;

public class CreateCommentValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Content).NotEmpty().WithMessage("Yorum boş olamaz.").MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir.");
    }
}
