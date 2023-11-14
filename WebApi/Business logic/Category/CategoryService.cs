using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebStore.Constants;
using WebStore.Data.Entitties.Identity;
using WebStore.Data.Entitties;
using WebStore.Models.Category;
using WebStore.Business_logic.Files;
using WebStore.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace WebStore.Business_logic.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IPictureService _pictureService;
        private readonly StoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<UserEntity> _userManager;

        public CategoryService(
            StoreDbContext context,
            IMapper mapper,
            IPictureService pictureService,
            UserManager<UserEntity> userManager
        )
        {
            _pictureService = pictureService;
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        private async Task<UserEntity?> GetCurrentUser(ClaimsPrincipal user)
            => await _userManager.GetUserAsync(user);

        private bool IsAdmin(UserEntity? user)
            => user != null && _userManager.IsInRoleAsync(user, Roles.Admin).Result;

        public async Task<IEnumerable<CategoryModel>> GetAll(ClaimsPrincipal user)
        {
            var userEntity = await GetCurrentUser(user);
            if (userEntity is null)
                throw new UnauthorizedAccessException("User not found.");

            var categories = await _context.Categories
                .Where(x => x.UserId == userEntity.Id || IsAdmin(userEntity)).ToListAsync();
            var mapped = _mapper.Map<IEnumerable<CategoryModel>>(categories);

            return mapped;
        }

        public async Task<CategoryModel> GetById(int id, ClaimsPrincipal user)
        {
            var userEntity = await GetCurrentUser(user);
            if (userEntity is null)
                throw new UnauthorizedAccessException("User not found.");

            var cat = await _context.Categories.FindAsync(id);

            if (cat is null || (userEntity?.Id != cat?.UserId && !IsAdmin(userEntity)))
                throw new Exception("Category not found.");

            var mapped = _mapper.Map<CategoryModel>(cat);
            return mapped;
        }

        public async Task<CategoryModel?> Create(CategoryCreateModel model, ClaimsPrincipal user)
        {
            try
            {
                var userEntity = await GetCurrentUser(user);
                if (userEntity is null)
                    throw new UnauthorizedAccessException();

                var cat = _mapper.Map<CategoryEntity>(model);

                // Save image
                string imageUrl = await _pictureService.Save(model.Image);
                cat.ImageUrl = imageUrl;

                // Add UserId
                cat.UserId = userEntity.Id;

                await _context.AddAsync(cat);
                await _context.SaveChangesAsync();

                var mapped = _mapper.Map<CategoryModel>(cat);
                return mapped;
            }
            catch
            {
                return null;
            }
        }

        public async Task<CategoryModel?> Update(CategoryUpdateModel model, ClaimsPrincipal user)
        {
            try
            {
                var userEntity = await GetCurrentUser(user);
                if (userEntity is null)
                    throw new UnauthorizedAccessException();

                var cat = await _context.Categories.FindAsync(model.Id)
                          ?? throw new Exception("Category not found.");

                if (cat.UserId != userEntity.Id && !IsAdmin(userEntity))
                    throw new UnauthorizedAccessException();

                _mapper.Map(model, cat);

                // Unlinking files and saving a new picture
                if (model.Image is not null)
                {
                    _pictureService.RemoveByUrl(cat.ImageUrl);
                    string newImageUrl = await _pictureService.Save(model.Image);
                    cat.ImageUrl = newImageUrl;
                }

                _context.Update(cat);
                await _context.SaveChangesAsync();

                var mapped = _mapper.Map<CategoryModel>(cat);
                return mapped;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> Delete(int id, ClaimsPrincipal user)
        {
            var cat = await _context.Categories.FindAsync(id);
            if (cat is null) return false;

            var userEntity = await GetCurrentUser(user);
            if (userEntity is null ||
                (cat.UserId != userEntity.Id && !IsAdmin(userEntity)))
                return false;

            // Unlinking files
            _pictureService.RemoveByUrl(cat.ImageUrl);

            _context.Remove(cat);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}