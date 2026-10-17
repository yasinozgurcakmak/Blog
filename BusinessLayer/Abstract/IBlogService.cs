using DtoLayer.Dtos.HomeDtos;
using EntityLayer.Concrete;
using System.Collections.Generic;

namespace BusinessLayer.Abstract
{
    public interface IBlogService : IGenericService<Blog>
    {
        PagedBlogCardsDto GetBlogCards(int? categoryId = null, int page = 1, int pageSize = 10);
        BlogDetailDto? GetBlog(string slug, bool countView, int? userId = null);
        BlogLikeResultDto? ToggleLike(int blogId, int userId);
    }
}
