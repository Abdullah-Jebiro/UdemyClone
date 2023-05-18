namespace Models.Dtos
{
    public class CourseForInstructorDto
    {

        public int CourseId { get; set; }
        public string Name { get; set; } = null!;
        public string About { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Price { get; set; }
        public string Level { get; set; } = null!;
        public string Language { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int VideosCount { get; set; }
        public int StudentsCount { get; set; }
        public int IsDelete { get; set; }

    }
}
