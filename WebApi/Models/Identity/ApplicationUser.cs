using Microsoft.AspNetCore.Identity;
using Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {       
        public string ProfilePictureUrl { get; set; } = "default.jpg"!;
        public string About { get; set; } = string.Empty;
        public string ResetPassword { get; set; } = string.Empty;
        public DateTime ResetPasswordExpiry { get; set; }=DateTime.MinValue;
        public List<ApplicationUserRole> UserRoles { get; set; } = null!;
        public List<UserCourses> UserCourses { get; set; } = null!;

    }
}
