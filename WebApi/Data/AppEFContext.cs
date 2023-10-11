using Microsoft.EntityFrameworkCore;
using WebApi.Data.Entitties;

namespace WebApi.Data
{
    public class AppEFContext : DbContext
    {
        public AppEFContext(DbContextOptions<AppEFContext> options)
            : base(options) { }
        public DbSet<CategoryEntity> Categories { get; set; }
    }
}
