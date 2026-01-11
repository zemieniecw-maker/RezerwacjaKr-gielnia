using Microsoft.AspNetCore.Identity;
using RezerwacjaKręgielnia.Entities;

namespace RezerwacjaKręgielnia.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<UserEntity>>();

            // 1. Tworzenie ról, jeśli nie istnieją
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Tworzenie konta Admina (jeśli nie istnieje)
            var adminEmail = "admin@kr.pl";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new UserEntity { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                // Tworzymy użytkownika z hasłem Admin123!
                var createResult = await userManager.CreateAsync(newAdmin, "Admin123!");

                if (createResult.Succeeded)
                {
                    // Przypisujemy mu rolę Admin
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}