using DtoLayer.Dtos.CommentDtos;
using System.Collections.Generic;

namespace BusinessLayer.Abstract
{
    public interface ICommentService
    {
        (List<CommentDto> Roots, int Count) GetCommentTree(int blogId, int? userId);
        int? AddComment(CreateCommentDto dto, int userId);
        bool DeleteComment(int commentId, int userId);
        CommentLikeResultDto? ToggleLike(int commentId, int userId);
        Dictionary<int, int> GetCommentCounts();
        int? DeleteCommentWithReplies(int commentId);
    }
}
