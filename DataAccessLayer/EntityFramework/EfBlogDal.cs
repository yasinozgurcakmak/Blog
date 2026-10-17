using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repository;
using EntityLayer.Concrete;
using DtoLayer.Dtos.HomeDtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.EntityFramework
{
    public class EfBlogDal(Context context) : GenericRepository<Blog>(context), IBlogDal
    {
        public List<BlogCardDto> GetBlogCards(int? categoryId, int skip, int take)
        {
            var now = DateTime.Now;
            var query =
                from b in context.Blogs.AsNoTracking()
                join c in context.Category.AsNoTracking() on b.CategoryId equals c.Id
                join u in context.Users.AsNoTracking() on b.AppUserId equals u.Id
                where b.Status && (b.PublishDate == null || b.PublishDate <= now)
                      && (categoryId == null || b.CategoryId == categoryId)
                orderby (b.PublishDate ?? b.CreatedDate) descending, b.Id descending
                select new BlogCardDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Summary = b.Summary,
                    Thumbnail = b.Thumbnail,
                    ReadingTime = b.ReadingTime,
                    PublishedAt = b.PublishDate ?? b.CreatedDate,
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    AuthorName = u.Name + " " + u.Surname,
                    AuthorImageUrl = u.ImageUrl
                };

            return query.Skip(skip).Take(take).ToList();
        }

        public BlogDetailDto? GetBlog(string slug)
        {
            var now = DateTime.Now;

            var query =
                from b in context.Blogs.AsNoTracking()
                join c in context.Category.AsNoTracking() on b.CategoryId equals c.Id
                join u in context.Users.AsNoTracking() on b.AppUserId equals u.Id
                where b.Slug == slug && b.Status && (b.PublishDate == null || b.PublishDate <= now)
                select new BlogDetailDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Slug = b.Slug,
                    Summary = b.Summary,
                    Content = b.Content,
                    Thumbnail = b.Thumbnail,
                    ReadingTime = b.ReadingTime,
                    ViewCount = b.ViewCount,
                    LikeCount = context.BlogLikes.Count(l => l.BlogId == b.Id),
                    MetaTitle = b.MetaTitle,
                    MetaDescription = b.MetaDescription,
                    PublishedAt = b.PublishDate ?? b.CreatedDate,
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    AuthorName = u.Name + " " + u.Surname,
                    AuthorImageUrl = u.ImageUrl,
                    AuthorHeadline = u.Headline,
                    AuthorBio = u.Bio,
                    AuthorGithubUrl = u.GithubUrl,
                    AuthorInstagramUrl = u.InstagramUrl,
                    AuthorXUrl = u.XUrl,
                    AuthorFacebookUrl = u.FacebookUrl,
                    AuthorWebsiteUrl = u.WebsiteUrl
                };

            return query.FirstOrDefault();
        }

       
        public void IncrementViewCount(int id)
        {
            context.Blogs.Where(b => b.Id == id).ExecuteUpdate(s => s.SetProperty(b => b.ViewCount, b => b.ViewCount + 1));
        }

        public bool IsPublished(int id)
        {
            var now = DateTime.Now;
            return context.Blogs.Any(b => b.Id == id && b.Status && (b.PublishDate == null || b.PublishDate <= now));
        }

        public bool HasLiked(int blogId, int userId) =>
            context.BlogLikes.Any(l => l.BlogId == blogId && l.AppUserId == userId);

        public bool ToggleLike(int blogId, int userId)
        {
            var removed = context.BlogLikes.Where(l => l.BlogId == blogId && l.AppUserId == userId).ExecuteDelete();
            if (removed > 0) return false;

            var like = new BlogLike { BlogId = blogId, AppUserId = userId, CreatedDate = DateTime.Now };
            context.BlogLikes.Add(like);
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

        public int GetLikeCount(int blogId) => context.BlogLikes.Count(l => l.BlogId == blogId);
    }
}
