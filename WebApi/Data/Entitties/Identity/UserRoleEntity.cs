using Microsoft.AspNetCore.Identity;

namespace WebStore.Data.Entitties.Identity
{
    public class UserRoleEntity : IdentityUserRole<int>
    {
        public UserEntity User { get; set; } = null!;
        public RoleEntity Role { get; set; } = null!;
    }
}