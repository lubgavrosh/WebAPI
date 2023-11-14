using System.Security.Claims;
using WebStore.Models.Category;

namespace WebStore.Business_logic.Category
{
    public interface ICategoryService
    {
        public Task<IEnumerable<CategoryModel>> GetAll(ClaimsPrincipal user);
        public Task<CategoryModel> GetById(int id, ClaimsPrincipal user);
        public Task<CategoryModel?> Create(CategoryCreateModel model, ClaimsPrincipal user);
        public Task<CategoryModel?> Update(CategoryUpdateModel model, ClaimsPrincipal user);
        public Task<bool> Delete(int id, ClaimsPrincipal user);
    }
}