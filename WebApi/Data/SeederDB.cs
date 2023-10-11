using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using WebApi.Data.Entitties;
using WebApi.Data;


namespace WebApi.Data
{
    public static class SeederDB
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppEFContext>();
                context.Database.Migrate();
                if (!context.Categories.Any())
                {
                    var laptop = new CategoryEntity
                    {
                        Name = "Ноутбуки",
                        Image = "https://image.coolblue.nl/content/0762aa167bbf3a08d1e11912d68be0fb",
                        Description = "Для роботи і навчання",
                        DateCreated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                    };

                    var clothes = new CategoryEntity
                    {
                        Name = "Одяг",
                        Image = "https://shoppingpl.com/uploads/post-covers/Polskie-marki-odziezy-damskiej-.jpg",
                        Description = "Для дорослих і малих",
                        DateCreated = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                    };

                    context.Categories.Add(laptop);
                    context.Categories.Add(clothes);
                    context.SaveChanges();
                }
            }
        }
    }
}