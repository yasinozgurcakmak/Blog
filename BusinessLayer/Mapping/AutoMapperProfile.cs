using AutoMapper;
using DtoLayer.Dtos.BlogDtos;
using DtoLayer.Dtos.CategoryDtos;
using DtoLayer.Dtos.UserDtos;
using EntityLayer.Concrete;

namespace BusinessLayer.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateBlogDto, Blog>()
            .ForMember(dest => dest.Thumbnail, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());

        CreateMap<UpdateBlogDto, Blog>()
            .ForMember(dest => dest.Thumbnail, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.ExistingThumbnail, opt => opt.MapFrom(src => src.Thumbnail))
            .ForMember(dest => dest.NewThumbnailFile, opt => opt.Ignore());

        CreateMap<Blog, BlogListDto>();

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<Category, CategoryListDto>().ReverseMap();


        CreateMap<AppUser, UserEditDto>()
            .ForMember(dest => dest.ExistingImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
    }
}
