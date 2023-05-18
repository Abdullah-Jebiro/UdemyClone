using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
