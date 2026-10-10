using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using DtoLayer.Dtos.HomeDtos;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete;

public class BlogManager(IBlogDal blogDal) : IBlogService
{
    private const int MaxPageSize = 50;

    public void TDelete(Blog t) => blogDal.Delete(t);
    public Blog TGetByID(int id) => blogDal.GetByID(id);
    public List<Blog> TGetList() => blogDal.GetList();
    public List<Blog> TGetListByFilter(Expression<Func<Blog, bool>> filter) => blogDal.GetListByFilter(filter);
    public void TInsert(Blog t) => blogDal.Insert(t);
    public void TMultiUpdate(List<Blog> t) => blogDal.MultiUpdate(t);
    public void TUpdate(Blog t) => blogDal.Update(t);

    public PagedBlogCardsDto GetBlogCards(int? categoryId = null, int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        
        var cards = blogDal.GetBlogCards(categoryId, (page - 1) * pageSize, pageSize + 1);

        return new PagedBlogCardsDto
        {
            HasMore = cards.Count > pageSize,
            Items = cards.Take(pageSize).ToList()
        };
    }

    public BlogDetailDto? GetBlog(string slug, bool countView)
    {
        if (string.IsNullOrWhiteSpace(slug)) return null;

        var detail = blogDal.GetBlog(slug);
        if (detail is null) return null;

        if (countView)
        {
            blogDal.IncrementViewCount(detail.Id);
            detail.ViewCount++;
        }
        return detail;
    }
}
