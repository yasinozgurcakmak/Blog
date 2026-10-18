namespace DtoLayer.Dtos.CommentDtos;

public class CreateCommentDto
{
    public int BlogId { get; set; }
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
}
