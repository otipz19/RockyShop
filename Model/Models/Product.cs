using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockyShop.Model.Models
{
	public class Product
	{
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        [DisplayName("Short Description")]
        [MinLength(1)]
        [MaxLength(100)]
        public string ShortDescription { get; set; }

        [MaxLength(200, ErrorMessage = "Max allowed length is 200 characters")]
        public string Description { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "From 1 to 10000")]
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
