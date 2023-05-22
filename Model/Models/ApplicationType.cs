using System.ComponentModel.DataAnnotations;

namespace RockyShop.Model.Models
{
    public class ApplicationType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(15)]
        public string Name { get; set; }
    }
}
