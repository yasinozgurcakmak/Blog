using DtoLayer.Dtos.CommentDtos;
using EntityLayer.Concrete;
using System.Collections.Generic;

namespace DataAccessLayer.Abstract
{
    public interface ICommentDal : IGenericDal<Comment>
    {
        List<CommentDto> GetBlogComments(int blogId, int? userId);
        bool ToggleLike(int commentId, int userId);
        int GetLikeCount(int commentId);
        Dictionary<int, int> GetCommentCounts();
        void DeleteByUser(int userId);
        void DeleteWithReplies(int commentId);
    }
}
