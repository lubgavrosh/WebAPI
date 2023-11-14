using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Account
{
    public class RegisterModel
    {
        [StringLength(100), Required]
        public string Email { get; set; } = String.Empty;
        [StringLength(100), Required]
        public string UserName { get; set; } = String.Empty;
        [StringLength(256), Required]
        public string Password { get; set; } = String.Empty;
        [StringLength(100), Required]
        public string FirstName { get; set; } = String.Empty;
        [StringLength(100), Required]
        public string LastName { get; set; } = String.Empty;
        public IFormFile? Image { get; set; }
    }
}