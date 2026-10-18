using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DtoLayer.Dtos.CommentDtos;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Concrete;

public class CommentManager(ICommentDal commentDal, IBlogDal blogDal) : ICommentService
{
    public (List<CommentDto> Roots, int Count) GetCommentTree(int blogId, int? userId)
    {
        var comments = commentDal.GetBlogComments(blogId, userId);
        var byId = comments.ToDictionary(c => c.Id);
        var roots = new List<CommentDto>();

        foreach (var comment in comments)
        {
            if (comment.ParentCommentId is int parentId && byId.TryGetValue(parentId, out var parent))
            {
                comment.ReplyToName = parent.IsDeleted ? null : parent.AuthorName;
                parent.Replies.Add(comment);
            }
            else
            {
                roots.Add(comment);
            }
        }

        roots.Reverse();

        RemoveDeletedLeaves(roots);
        return (roots, comments.Count(c => !c.IsDeleted));
    }

    public int? AddComment(CreateCommentDto dto, int userId)
    {
        if (!blogDal.IsPublished(dto.BlogId)) return null;

        if (dto.ParentCommentId is int parentId)
        {
            var parent = commentDal.GetByID(parentId);
            if (parent is null || parent.BlogId != dto.BlogId || parent.IsDeleted) return null;
        }

        var comment = new Comment
        {
            BlogId = dto.BlogId,
            AppUserId = userId,
            ParentCommentId = dto.ParentCommentId,
            Content = dto.Content.Trim(),
            CreatedDate = DateTime.Now
        };
        commentDal.Insert(comment);
        return comment.Id;
    }

    public bool DeleteComment(int commentId, int userId)
    {
        var comment = commentDal.GetByID(commentId);
        if (comment is null || comment.AppUserId != userId || comment.IsDeleted) return false;

        comment.IsDeleted = true;
        commentDal.Update(comment);
        return true;
    }

    public CommentLikeResultDto? ToggleLike(int commentId, int userId)
    {
        var comment = commentDal.GetByID(commentId);
        if (comment is null || comment.IsDeleted || !blogDal.IsPublished(comment.BlogId)) return null;

        var liked = commentDal.ToggleLike(commentId, userId);
        return new CommentLikeResultDto { Liked = liked, LikeCount = commentDal.GetLikeCount(commentId) };
    }

    public Dictionary<int, int> GetCommentCounts() => commentDal.GetCommentCounts();

    public int? DeleteCommentWithReplies(int commentId)
    {
        var comment = commentDal.GetByID(commentId);
        if (comment is null) return null;

        commentDal.DeleteWithReplies(commentId);
        return comment.BlogId;
    }

    private static void RemoveDeletedLeaves(List<CommentDto> comments)
    {
        foreach (var comment in comments) RemoveDeletedLeaves(comment.Replies);
        comments.RemoveAll(c => c.IsDeleted && c.Replies.Count == 0);
    }
}
