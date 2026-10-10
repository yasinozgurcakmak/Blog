namespace DtoLayer.Dtos.HomeDtos;

public class BlogCardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Summary { get; set; }
    public string? Thumbnail { get; set; }
    public int ReadingTime { get; set; }
    public DateTime PublishedAt { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorImageUrl { get; set; }
}
