namespace Models.Dtos
{


    public class CourseDto
    {
      
        public int CourseId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public double Price { get; set; }
        public string About { get; set; } = null!;
        public string Level { get; set; } = null!;
        public string Language { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
        public string UserName { get; set; }
        public int CategoryId { get; set; }
        public int VideosCount { get; set; }

    }
}
