using Microsoft.AspNet.Identity;
using Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Models.DbEntities
{
    public class UserCourses
    {
        public int UserCoursesId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public Course Course { get; set; }= null!;


    }
}
