using RockyShop.Utility.Interfaces;

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
                .AddAppDbContext(builder.Configuration, builder.Environment)
                .AddMyRepositories()
                .AddMyServices()
                .AddMyOptions(builder.Configuration)
                .AddSessionAccessor();

            builder.Services.AddIdentityServices();

            builder.Services.AddFacebookAuthentication(builder.Configuration);

            var app = builder.Build();

            using(var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetService<IDbInitializer>().Initialize().GetAwaiter().GetResult();
            }

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