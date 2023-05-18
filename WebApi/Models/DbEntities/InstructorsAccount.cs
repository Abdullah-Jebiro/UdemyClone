using Models.Identity;

namespace Models.DbEntities
{
    public class InstructorAccount
    {
        public int UserId { get; set; }
        public double Account { get; set; }
        public ApplicationUser User { get; set; }


    }
}
