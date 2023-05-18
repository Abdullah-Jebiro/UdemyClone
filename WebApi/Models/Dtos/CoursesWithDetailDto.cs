namespace Models.Dtos
{
    public class CoursesWithDetailDto
    {
        public int CourseId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Price { get; set; }
        public string About { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
        public int VideosCount { get; set; }
        public int StudentsCount { get; set; }
        public string Status { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string Language { get; set; } = null!;
        public int InstructorId { get; set; } 
        public string Instructor { get; set; }=null!;
        public string ProfilePictureUrl { get; set; } = null!;
        public int StudentsCountForInstructor { get; set; }
        public int CoursesCountForInstructor { get; set; }

    }
}
