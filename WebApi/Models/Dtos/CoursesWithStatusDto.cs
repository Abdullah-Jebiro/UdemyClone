namespace Models.Dtos
{
    public class CoursesWithStatusDto
    {
        public int CategoryId { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public string About { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;   
        public int VideosCount { get; set; }
        public string Status { get; set; } = null!;

    }
}
