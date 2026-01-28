using HomeTech.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HomeTech.Infrastructure
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in new[] { Roles.Admin, Roles.Manager, Roles.User })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            await CreateUserAsync(
                userManager,
                "a7med.2001.bahej@gmail.com",
                "Admin, Ahmed Gamal Gaber",
                Roles.Admin,
                "946201.Com.Com"
            );

            await CreateUserAsync(
                userManager,
                "hakeamsoma@gmail.com",
                "Manaegr, Ismail Abd ElHakeem",
                Roles.Manager,
                "Mahmoud123"
            );

            await CreateUserAsync(
                userManager,
                "zeko80324@gmail.com",
                "Manaegr, Mahmoud fawzy fahmy",
                Roles.Manager,
                "Mahmoud132001"
            );

            await CreateUserAsync(
                userManager,
                "abdo.mostafa69444@gmail.com",
                "Manaegr, Abdo Mostafa Samna",
                Roles.Manager,
                "Abdo1234@"
            );
        }

        private static async Task CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string name,
            string role,
            string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null) return;

            user = new ApplicationUser
            {
                Email = email,
                FullName = name,
                IsActive = true,
                UserName = email,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
        }
    }
}