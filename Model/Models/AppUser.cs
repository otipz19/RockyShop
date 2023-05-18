using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RockyShop.Model.Models
{
    public class AppUser : IdentityUser
    {
        [DisplayName("Full Name")]
        [Required]
        public string FullName { get; set; }

        [NotMapped]
        [Required]
        public string State { get; set; }

        [NotMapped]
        [Required]
        public string City { get; set; }

        [NotMapped]
        [Required]
        public string Street { get; set; }

        [NotMapped]
        [Required]
        [DisplayName("Poastal code")]
        public string PoastalCode { get; set; }
    }
}
