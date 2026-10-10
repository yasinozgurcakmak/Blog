namespace DtoLayer.Dtos.HomeDtos;

public class BlogDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Summary { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Thumbnail { get; set; }
    public int ReadingTime { get; set; }
    public int ViewCount { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public DateTime PublishedAt { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorImageUrl { get; set; }
}
