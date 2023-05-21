using RockyShop.DataAccess.Repository.Interfaces;
using RockyShop.DataAccess.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using RockyShop.Utility.Interfaces;
using RockyShop.Utility.Services;
using RockyShop.Utility.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using RockyShop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace RockyShop
{
    public static class MyServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            return services.AddDbContext<AppDbContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(configuration
                    .GetConnectionString(environment.IsDevelopment() ? "Localhost" : "Azure"));
            });
        }

        public static IServiceCollection AddMyRepositories(this IServiceCollection services)
        {
            return services.AddScoped<ICategoryRepository, CategoryRepository>()
                .AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>()
                .AddScoped<IInquiryDetailsRepository, InquiryDetailsRepository>()
                .AddScoped<IAppUserRepository, AppUserRepository>()
                .AddScoped<IOrderHeaderRepository, OrderHeaderRepository>()
                .AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();
        }

        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            return services.AddScoped<ProductImageService>()
                .AddScoped<ShoppingCartService>()
                .AddTransient<IEmailSenderService, EmailSenderService>()
                //For Identity Razor Pages
                .AddTransient<IEmailSender, EmailSenderService>()
                .AddScoped<IBraintreeService, BraintreeService>()
                .AddScoped<IDbInitializer, DbInitializer>();
        }

        public static IServiceCollection AddMyOptions(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<BraintreeSettings>(configuration.GetSection(BraintreeSettings.Section))
                .Configure<MailjetSettings>(configuration.GetSection(MailjetSettings.Section));
        }

        public static AuthenticationBuilder AddFacebookAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddAuthentication().AddFacebook(options =>
            {
                var section = configuration.GetSection("Facebook");
                options.AppId = section["AppId"];
                options.AppSecret = section["AppSecret"];
            });
        }

        public static IdentityBuilder AddIdentityServices(this IServiceCollection services)
        {
            return services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<AppDbContext>();
        }

        public static IServiceCollection AddSessionAccessor(this IServiceCollection services)
        {
            return services.AddHttpContextAccessor()
                .AddSession(options =>
                {
                    options.Cookie.IsEssential = true;
                    options.IdleTimeout = TimeSpan.FromMinutes(10);
                    options.Cookie.HttpOnly = true;
                });
        }
    }
}
