using Models.Core;

namespace Models.DbEntities
{
    public class Category: SoftDeletable
    {
        public int CategoryId { get; set; }
        public string nameCategory { get; set; } = null!;
        public List<Course> Courses { get; set; } = new List<Course>();
    }
}