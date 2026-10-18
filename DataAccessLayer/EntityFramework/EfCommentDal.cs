using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repository;
using DtoLayer.Dtos.CommentDtos;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.EntityFramework
{
    public class EfCommentDal(Context context) : GenericRepository<Comment>(context), ICommentDal
    {
        public List<CommentDto> GetBlogComments(int blogId, int? userId)
        {
            var query =
                from c in context.Comments.AsNoTracking()
                join u in context.Users.AsNoTracking() on c.AppUserId equals u.Id
                where c.BlogId == blogId
                orderby c.CreatedDate, c.Id
                select new CommentDto
                {
                    Id = c.Id,
                    ParentCommentId = c.ParentCommentId,
                    Content = c.IsDeleted ? string.Empty : c.Content,
                    CreatedDate = c.CreatedDate,
                    IsDeleted = c.IsDeleted,
                    AuthorName = u.Name + " " + u.Surname,
                    AuthorImageUrl = u.ImageUrl,
                    AuthorHeadline = u.Headline,
                    LikeCount = context.CommentLikes.Count(l => l.CommentId == c.Id),
                    IsLiked = userId != null && context.CommentLikes.Any(l => l.CommentId == c.Id && l.AppUserId == userId),
                    IsOwner = userId != null && c.AppUserId == userId
                };

            return query.ToList();
        }

        public bool ToggleLike(int commentId, int userId)
        {
            var removed = context.CommentLikes.Where(l => l.CommentId == commentId && l.AppUserId == userId).ExecuteDelete();
            if (removed > 0) return false;

            var like = new CommentLike { CommentId = commentId, AppUserId = userId, CreatedDate = DateTime.Now };
            context.CommentLikes.Add(like);
            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                context.Entry(like).State = EntityState.Detached;
            }
            return true;
        }

        public int GetLikeCount(int commentId) => context.CommentLikes.Count(l => l.CommentId == commentId);

        public Dictionary<int, int> GetCommentCounts() =>
            context.Comments.Where(c => !c.IsDeleted).GroupBy(c => c.BlogId).Select(g => new { BlogId = g.Key, Count = g.Count() }).ToDictionary(x => x.BlogId, x => x.Count);

        public void DeleteByUser(int userId)
        {
            context.CommentLikes.Where(l => l.AppUserId == userId).ExecuteDelete();
            DeleteWithReplies(context.Comments.Where(c => c.AppUserId == userId).Select(c => c.Id).ToList());
        }

        public void DeleteWithReplies(int commentId) => DeleteWithReplies(new List<int> { commentId });

        private void DeleteWithReplies(List<int> commentIds)
        {
            var frontier = commentIds;
            var ids = new HashSet<int>(frontier);
            while (frontier.Count > 0)
            {
                var parents = frontier;
                frontier = context.Comments.Where(c => c.ParentCommentId != null && parents.Contains(c.ParentCommentId.Value)).Select(c => c.Id).ToList().Where(ids.Add).ToList();
            }

            context.Comments.Where(c => ids.Contains(c.Id)).ExecuteDelete();
        }
    }
}
