namespace Models.Dtos
{
    public class CoursesForUserDto
    {
        public int CourseId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Price { get; set; }
        public string About { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;

    }
}
