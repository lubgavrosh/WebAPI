using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebStore.Data.Entitties;

namespace WebApi.Data.Entitties
{

    [Table("tblCategories")]
    public class CategoryEntity:BaseEntity<int>

    {
        [Required, StringLength(255)]
        public string Name { get; set; }


        [Required, StringLength(255)]
        public string Image { get; set; }


        [StringLength(4000)] 
        public string Description { get; set; }

   
    }
}
