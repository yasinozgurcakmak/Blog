using EntityLayer.Concrete;
using DtoLayer.Dtos.HomeDtos;
using System.Collections.Generic;

namespace DataAccessLayer.Abstract
{
    public interface IBlogDal : IGenericDal<Blog>
    {
        List<BlogCardDto> GetBlogCards(int? categoryId, int skip, int take);
        BlogDetailDto? GetBlog(string slug);
        void IncrementViewCount(int id);
    }
}
