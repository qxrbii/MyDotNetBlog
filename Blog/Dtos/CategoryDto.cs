namespace Blog.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; } // 用來做連結
        public int PostCount { get; set; } //用來顯示數量
    }
}
