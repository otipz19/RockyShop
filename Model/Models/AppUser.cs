using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace RockyShop.Model.Models
{
    public class AppUser : IdentityUser
    {
        [DisplayName("Full Name")]
        public string FullName { get; set; }
    }
}
