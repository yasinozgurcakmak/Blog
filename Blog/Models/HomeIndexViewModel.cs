using DtoLayer.Dtos.HomeDtos;

namespace Blog.Models;

public class HomeIndexViewModel
{
    public List<BlogCardDto> Blogs { get; set; } = [];
    public bool HasMore { get; set; }
    public List<CategoryWithCountDto> Categories { get; set; } = [];
    public int? SelectedCategoryId { get; set; }
}
