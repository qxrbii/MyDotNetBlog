using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Posts
    {
        public int Id { get; set; }                 // 主鍵
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;          // 文章標題

        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;            // 文章 URL Slug

        [Required]
        public string? Content { get; set; }         // 文章內容

        [Required]
        public DateTime PublishDate { get; set; } // 發布日期

        public string? Author { get; set; } // 作者

        [MaxLength(500)]
        public string Excerpt { get; set; } = string.Empty;     // 文章摘要

        public ICollection<PostImage> Images { get; set; } = new List<PostImage>();

        public int ViewCount { get; set; }      // 瀏覽次數

        public int? CategoryId { get; set; }        // 外鍵，指向分類
        public Category? Category { get; set; }     //  導航屬性，指向分類

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

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
