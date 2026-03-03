namespace Blog.Dtos
{
    public class AdminPostListDto
    {
        public int Id { get; set; }         // 後台刪除通常還是用 ID 比較穩
        public string Title { get; set; }
        public string Slug { get; set; }    // 為了產生 /Admin/EditPost/slug 連結
        public string CategoryName { get; set; }
        public string Author { get; set; }     // 看作者是誰
        public DateTime PublishDate { get; set; }
        public int ViewCount { get; set; }

        public string DisplayAuther
        {
            get
            {
                return Author == "Admin" ? "站長" : (Author ?? "匿名");
            }
        }
    }
}
