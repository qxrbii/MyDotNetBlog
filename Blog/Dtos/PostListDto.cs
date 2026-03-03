namespace Blog.Dtos
{
    public class PostListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string CategoryName { get; set; }
        public DateTime PublishDate { get; set; }
        public int ViewCount { get; set; }
        public string Author { get; set; } // 存儲作者名稱
        public string Excerpt { get; set; }     // 文章摘要

        public string DisplayAuther
        {
            get
            {
                // 1. 先判斷是否為空
                if (string.IsNullOrWhiteSpace(Author)) return "匿名";

                // 2. 移除空格並忽略大小寫比對
                if (Author.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return "站長";
                }

                return Author;
            }
        }
    }
}
