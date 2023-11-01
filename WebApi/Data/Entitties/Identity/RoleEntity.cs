using Microsoft.AspNetCore.Identity;

namespace WebStore.Data.Entitties.Identity
{
    public class RoleEntity : IdentityRole<int>
    {
        public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
    }
}