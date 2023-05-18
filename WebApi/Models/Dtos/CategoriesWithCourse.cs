namespace Models.Dtos
{
    public class CategoriesWithCourse
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public List<CoursesWithStatusDto> Courses { get; set; } = null!;
    }
}