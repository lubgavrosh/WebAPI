using System.ComponentModel.DataAnnotations;

namespace WebStore.Models.Category
{
    public class CategoryCreateModel
    {
        [StringLength(100), Required]
        public string Name { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        [StringLength(500), Required]
        public string Description { get; set; } = string.Empty;
    }
}