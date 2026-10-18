namespace DtoLayer.Dtos.CommentDtos;

public class CommentDto
{
    public int Id { get; set; }
    public int? ParentCommentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsDeleted { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorImageUrl { get; set; }
    public string? AuthorHeadline { get; set; }
    public int LikeCount { get; set; }
    public bool IsLiked { get; set; }
    public bool IsOwner { get; set; }
    public string? ReplyToName { get; set; }
    public List<CommentDto> Replies { get; set; } = new();
}
