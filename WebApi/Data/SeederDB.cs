using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using WebStore.Data.Entitties;
using WebStore.Constants;
using WebStore.Data.Entitties.Identity;

namespace WebStore.Data
{
    public static class SeederDB
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppEFContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
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

                if (!context.Roles.Any())
                {
                    RoleEntity admin = new RoleEntity
                    {
                        Name = Roles.Admin
                    };
                    RoleEntity user = new RoleEntity
                    {
                        Name = Roles.User
                    };
                    var result = roleManager.CreateAsync(admin).Result;
                    if (result.Succeeded)
                    {
                        Console.WriteLine("----Роль успішно створено---- {0}", admin.Name);
                    }
                    else
                    {
                        Console.WriteLine("+++Помилка створення ролі+++ {0}", admin.Name);
                    }
                    result = roleManager.CreateAsync(user).Result;
                    if (result.Succeeded)
                    {
                        Console.WriteLine("----Роль успішно створено---- {0}", user.Name);
                    }
                    else
                    {
                        Console.WriteLine("+++Помилка створення ролі+++ {0}", user.Name);
                    }


                }

                if (!context.Users.Any())
                {
                    var ivan = new UserEntity
                    {
                        FirstName = "Марко",
                        LastName = "Лисий",
                        Email = "lysyi@gmail.com",
                        UserName = "lysyi@gmail.com"
                    };
                    var result = userManager.CreateAsync(ivan, "123456").Result;
                    if (result.Succeeded)
                    {
                        Console.WriteLine("-------Користувача успішно створено-------- {0}", ivan.Email);
                        result = userManager.AddToRoleAsync(ivan, Roles.Admin).Result;
                        if (result.Succeeded) { Console.WriteLine("---Користувач {0} отримав роль {1}----", ivan.Email, Roles.Admin); }
                        else { Console.WriteLine("++++Помилка видчаі ролі {1} Користувачу {0}++++", ivan.Email, Roles.Admin); }
                    }
                    else
                    {
                        Console.WriteLine("+++++Помилка створення користувача++++++ {0}", ivan.Email);
                    }
                }
            }
        }
    }
}