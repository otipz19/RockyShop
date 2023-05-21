using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RockyShop.Utility.Interfaces;
using RockyShop.DataAccess.Data;
using RockyShop.Utility.Utilities;
using RockyShop.Model.Models;

namespace RockyShop.Utility.Services
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(AppDbContext dbContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            if (_dbContext.Database.GetPendingMigrations().Any())
            {
                _dbContext.Database.Migrate();
            }

            if (!await _roleManager.RoleExistsAsync(Constants.AdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(Constants.AdminRole));
                await _roleManager.CreateAsync(new IdentityRole(Constants.CustomerRole));

                await _userManager.CreateAsync(new AppUser()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true,
                    FullName = "Admin Admin",
                    PhoneNumber = "admin",
                }, "Admin123*");

                AppUser admin = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.Email == "admin@admin.com");
                await _userManager.AddToRoleAsync(admin, Constants.AdminRole);
            }
        }
    }
}
