using RockyShop.Data;
using Microsoft.EntityFrameworkCore;
using RockyShop.Services;
using Microsoft.AspNetCore.Identity;
using RockyShop.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace RockyShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

            builder.Services
                .AddDbContext<AppDbContext>(optionsBuilder =>
                {
                    optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
                });

            builder.Services
                .AddHttpContextAccessor()
                .AddSession(options =>
                {
                    options.Cookie.IsEssential = true;
                    options.IdleTimeout = TimeSpan.FromMinutes(10);
                    options.Cookie.HttpOnly = true;
                });

            builder.Services
                .AddScoped<ProductImageService>()
                .AddScoped<ShoppingCartService>()
                .AddTransient<IEmailSenderService, EmailSenderService>()
                //For Identity Razor Pages
                .AddTransient<IEmailSender, EmailSenderService>();

            builder.Services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<AppDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}/{keyword?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}