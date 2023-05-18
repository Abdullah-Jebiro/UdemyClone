using Models.Identity;

namespace Models.DbEntities
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Price { get; set; }
        public string About { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int LevelId { get; set; }
        public int LanguageId { get; set; }
        public bool IsDelete { get; set; } = false;
        public Language Language { get; set; } = null!;
        public Level Level { get; set; } = null!;
        public List<Video> Videos { get; set; } = new List<Video>();
        public List<UserCourses> UserCourses { get; set; } = new List<UserCourses>();
        public Category Category { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

    }
}
