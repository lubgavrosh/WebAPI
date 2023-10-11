using AutoMapper;
using WebApi.Data.Entitties;
using WebApi.Models.Category;

namespace WebApi.Mapper
{
    public class AppMapProfile : Profile
    {
        public AppMapProfile()
        {
            CreateMap<CategoryEntity, CategoryItemViewModel>();
            //.ForMember(x => x.Image, opt => opt.MapFrom(x => $"/images/{x.Image}"));
        }
    }
}