using AutoMapper;
using WebStore.Data.Entitties;
using WebStore.Models.Category;
using WebStore.Data.Entitties;
using WebStore.Models.Category;
using WebStore.Data.Entitties.Identity;
using WebStore.Models.Account;

namespace WebStore.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CategoryEntity, CategoryModel>()
                .ForMember(x => x.Image, opt => opt.MapFrom(src => src.ImageUrl));

            CreateMap<CategoryModel, CategoryEntity>()
                .ForMember(x => x.ImageUrl, opt => opt.MapFrom(src => src.Image));

            CreateMap<CategoryCreateModel, CategoryEntity>().ForMember(
                x => x.ImageUrl,
                opt => opt.Ignore()
            );
            CreateMap<CategoryUpdateModel, CategoryEntity>().ForMember(
                x => x.ImageUrl,
                opt => opt.Ignore()
            );

            CreateMap<RegisterModel, UserEntity>().ForMember(
                x => x.Image,
                opt => opt.Ignore()
            );
        }
    }
}