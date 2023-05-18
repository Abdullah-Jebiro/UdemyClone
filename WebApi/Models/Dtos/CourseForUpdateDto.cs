namespace Models.Dtos
{
    public class CourseForUpdateDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Price { get; set; }
        public string About { get; set; } = null!;
        public int LevelId { get; set; }
        public int LanguageId { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
        public int CategoryId { get; set; }
    }
}
