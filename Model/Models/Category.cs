using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RockyShop.Model.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(15)]
        public string Name { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Display order should be positive!")]
        [DisplayName("Display order")]
        public int DisplayOrder { get; set; } 
    }
}
