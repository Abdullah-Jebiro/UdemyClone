using Models.Identity;

namespace Models.DbEntities
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
