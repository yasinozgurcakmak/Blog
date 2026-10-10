using DtoLayer.Dtos.HomeDtos;
using EntityLayer.Concrete;
using System.Collections.Generic;

namespace BusinessLayer.Abstract
{
    public interface ICategoryService : IGenericService<Category>
    {
        List<CategoryWithCountDto> GetPopularCategories(int take = 5);
    }
}
