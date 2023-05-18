using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Seeds
{
    public static class DefaultSuperAdmin
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            // Default User
            if (!userManager.Users.Any())
            {            
                var defaultUser = new ApplicationUser
                {
                    UserName = "abdullahjbero",
                    Email = "abdullahjbero@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                await userManager.CreateAsync(defaultUser, "abdullahjbero123!");            
                await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
                await userManager.AddToRoleAsync(defaultUser, Roles.Moderator.ToString());
                await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
            }
        }
    }
}