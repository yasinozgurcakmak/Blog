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
    public class EfCategoryDal(Context context) : GenericRepository<Category>(context), ICategoryDal
    {
        public List<CategoryWithCountDto> GetCategoriesWithPublishedBlogCount(int take)
        {
            var now = DateTime.Now;

            return context.Category.AsNoTracking()
                .Select(c => new CategoryWithCountDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    BlogCount = context.Blogs.Count(b => b.CategoryId == c.Id && b.Status && (b.PublishDate == null || b.PublishDate <= now))
                })
                .Where(x => x.BlogCount > 0)
                .OrderByDescending(x => x.BlogCount)
                .ThenBy(x => x.Name)
                .Take(take)
                .ToList();
        }
    }
}
