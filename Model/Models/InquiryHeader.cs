using System.ComponentModel.DataAnnotations;

namespace RockyShop.Model.Models
{
    public class InquiryHeader
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AppUserId { get; set; }

        public virtual AppUser AppUser { get; set; }

        public DateTime InquiryTime { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }
    }
}
