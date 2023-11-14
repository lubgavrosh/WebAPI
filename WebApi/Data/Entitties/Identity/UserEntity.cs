using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace WebStore.Data.Entitties.Identity
{

    public class UserEntity : IdentityUser<int>
    {
        [StringLength(100), Required]
        public string FirstName { get; set; } = String.Empty;
        [StringLength(100), Required]
        public string LastName { get; set; } = String.Empty;

        [StringLength(300)]
        public string? Image { get; set; }

        public virtual ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();

        public virtual ICollection<CategoryEntity> Categories { get; set; } = new List<CategoryEntity>();

    }
}