using DtoLayer.Dtos.BlogDtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Areas.blgpnl.Models;


public abstract class BlogFormViewModelBase
{
    public IEnumerable<SelectListItem> Categories { get; set; } = [];
}

public sealed class BlogCreateViewModel : BlogFormViewModelBase
{
    public CreateBlogDto Dto { get; set; } = new();
}

public sealed class BlogEditViewModel : BlogFormViewModelBase
{
    public UpdateBlogDto Dto { get; set; } = new();
}
