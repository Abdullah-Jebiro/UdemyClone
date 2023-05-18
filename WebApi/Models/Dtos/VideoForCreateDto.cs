namespace Models.Dtos
{
    public class VideoForCreateDto
    {
        public string VideoTitle { get; set; } = null!;
        public string VideoUrl { get; set; } = null!;
        public int CourseId { get; set; }
    }
}
