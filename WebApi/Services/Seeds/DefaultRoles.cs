using Microsoft.AspNetCore.Identity;
using Models.Enums;
using Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
        {
            //Default Roles
            await roleManager.CreateAsync(new ApplicationRole()
            {
                Name = Roles.SuperAdmin.ToString(),
                CreatedDate = DateTime.Now
            });
            await roleManager.CreateAsync(new ApplicationRole()
            {
                Name = Roles.Admin.ToString(),
                CreatedDate = DateTime.Now
            });
            await roleManager.CreateAsync(new ApplicationRole()
            {
                Name = Roles.Moderator.ToString(),
                CreatedDate = DateTime.Now
            });
            await roleManager.CreateAsync(new ApplicationRole()
            {
                Name = Roles.Basic.ToString(),
                CreatedDate = DateTime.Now
            });
        }
    }
}
