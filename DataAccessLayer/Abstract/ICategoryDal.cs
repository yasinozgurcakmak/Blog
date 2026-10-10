using EntityLayer.Concrete;
using DtoLayer.Dtos.HomeDtos;
using System.Collections.Generic;

namespace DataAccessLayer.Abstract
{
    public interface ICategoryDal : IGenericDal<Category>
    {
        List<CategoryWithCountDto> GetCategoriesWithPublishedBlogCount(int take);
    }
}
