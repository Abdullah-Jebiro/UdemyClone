namespace Models.Dtos
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string About { get; set; } = null!;
        public string ThumbnailUrl { get; set; } = null!;
    }
}