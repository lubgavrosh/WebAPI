using AutoMapper;
using WebApi.Data.Entitties;
using WebApi.Models.Category;
using WebStore.Data.Entitties;
using WebStore.Models.Category;

namespace WebStore.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            CreateMap<CategoryEntity, CategoryItemViewModel>();
            //.ForMember(x => x.Image, opt => opt.MapFrom(x => $"/images/{x.Image}"));

            CreateMap<CategoryCreateViewModel, CategoryEntity>();
        }
    }
}