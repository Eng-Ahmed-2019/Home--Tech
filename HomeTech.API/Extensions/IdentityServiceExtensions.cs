using HomeTech.Domain.Entities;
using HomeTech.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace HomeTech.API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityConfiguration(
            this IServiceCollection services
        )
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 4;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.AllowedForNewUsers = true;
            })
                .AddEntityFrameworkStores<HomeTechDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}