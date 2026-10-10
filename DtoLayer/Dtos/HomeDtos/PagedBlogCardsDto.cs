namespace DtoLayer.Dtos.HomeDtos;

public class PagedBlogCardsDto
{
    public List<BlogCardDto> Items { get; set; } = [];
    public bool HasMore { get; set; }
}
