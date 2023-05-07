using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RockyShop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Display order should be positive!")]
        [DisplayName("Display order")]
        public int DisplayOrder { get; set; } 
    }
}
