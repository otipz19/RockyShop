using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockyShop.Models
{
	public class Product
	{
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be positive")]
        public double Price { get; set; }

        public string Image { get; set; }

        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [DisplayName("Application Type")]
        public int ApplicationTypeId { get; set; }

        [ForeignKey("ApplicationTypeId")]
        public virtual ApplicationType ApplicationType { get; set; }
    }
}
