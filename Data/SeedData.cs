using Microsoft.AspNetCore.Identity;
//using DEV1_2024_Assignment.Models;



    public class SeedData
    {
        public static async Task InitializeAsync(UserManager<Cliente> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Cliente" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            if (await userManager.FindByEmailAsync("admin@admin.com") == null)
            {
                var adminUser = new Cliente
                {
                    Nome = "Admin",
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Password1@");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddToRoleAsync(adminUser, "Cliente");
                    
                }
            }
            else
            {
                var adminUser = await userManager.FindByEmailAsync("admin@admin.com");
                await userManager.AddToRoleAsync(adminUser, "Admin");
                await userManager.AddToRoleAsync(adminUser, "Cliente");
               
            }
        }
    }
