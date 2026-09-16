using System;

namespace DtoLayer.Dtos.BlogDtos;

public class BlogListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Summary { get; set; }
    public string? Thumbnail { get; set; }
    public int ReadingTime { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
