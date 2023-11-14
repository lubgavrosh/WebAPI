using System.ComponentModel.DataAnnotations;

namespace WebStore.Data.Entitties.Identity
{
    public class UserRefreshTokens
    {
        [Key] public int Id { get; set; }
        [Required] public string UserName { get; set; } = String.Empty;
        [Required] public string RefreshToken { get; set; } = String.Empty;
        public bool IsActive { get; set; } = true;
    }
}
