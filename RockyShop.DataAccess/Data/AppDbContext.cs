using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RockyShop.Model.Models;

namespace RockyShop.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ApplicationType> ApplicationTypes { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<InquiryHeader> InquiryHeaders { get; set; }

        public DbSet<InquiryDetails> InquiryDetails { get; set; }

        public DbSet<OrderHeader> OrderHeaders { get; set; }

        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}
