using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using WebStore.Data.Entitties;
using WebStore.Constants;
using WebStore.Data.Entitties.Identity;
using WebStore.Business_logic.Files;
using WebStore.Data.Context;

namespace WebStore.Data.Seeder
{
    public static class DbSeeder
    {
       
    private static IPictureService _pictureService;
        static DbSeeder()
        {
            _pictureService = new LocalPictureService();
        }

        public static async Task SeedDatabase(this IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();

            await MigrateIfAnyIn(ctx);

            if (!await ctx.Roles.AnyAsync())
                await SeedRoles(roleManager);

            if (!await ctx.Users.AnyAsync())
                await SeedUsers(userManager);

            if (!await ctx.Categories.AnyAsync())
                await SeedCategoriesInto(ctx);
        }

        private static async Task MigrateIfAnyIn(DbContext ctx)
        {
            if ((await ctx.Database.GetPendingMigrationsAsync()).Any())
                await ctx.Database.MigrateAsync();
        }

        private static async Task SeedCategoriesInto(DbContext ctx)
        {
            Uri[] pics =
            {
            new("https://content2.rozetka.com.ua/goods/images/big/364772063.jpg"),
            new("https://content1.rozetka.com.ua/goods/images/big/345070364.jpg"),
            new("https://content2.rozetka.com.ua/goods/images/big/348227920.jpg"),
            new("https://images.ctfassets.net/5de70he6op10/2Ri6bD1TeMmWConq9ob9pH/a7ed1cd3039b77e99c5fcab65b88b844/Decor_Gateway_LS_01_b.jpg?w=1752&q=80&fm=jpg&fl=progressive"),
        };

            List<string> savedPics = new();
            foreach (var pic in pics)
                savedPics.Add(await _pictureService.Save(pic));

            var categories = new List<CategoryEntity>
        {
            new() {
                Name = "Electronics",
                ImageUrl = savedPics[0],
                UserId = 1,
                Description = "Explore the latest in electronic gadgets and devices."
            },
            new()
            {
                Name = "Clothing",
                ImageUrl = savedPics[1],
                UserId = 1,
                Description = "Discover fashionable clothing for all occasions."
            },
            new()
            {
                Name = "Books",
                ImageUrl = savedPics[2],
                UserId = 1,
                Description = "Dive into a world of books, from classics to bestsellers."
            },
            new()
            {
                Name = "Home Decor",
                ImageUrl = savedPics[3],
                UserId = 1,
                Description = "Transform your living space with stylish home decor."
            }
        };

            foreach (var c in categories)
                await ctx.AddAsync(c);

            await ctx.SaveChangesAsync();
        }

        private static async Task SeedRoles(
            RoleManager<RoleEntity> roleManager)
        {
            var admin = new RoleEntity { Name = Roles.Admin };
            var user = new RoleEntity { Name = Roles.User };

            await roleManager.CreateAsync(admin);
            await roleManager.CreateAsync(user);
        }

        private static async Task SeedUsers(
            UserManager<UserEntity> userManager)
        {
            var admin = new UserEntity
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                FirstName = "Marko",
                LastName = "Lysyi",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, "@Adminpwd123");
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }
}