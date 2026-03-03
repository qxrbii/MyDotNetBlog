using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Category
    {
        public int Id { get; set; }     // 主鍵

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;    // 分類名稱

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;    // 分類 URL Slug

        public ICollection<Posts> Posts { get; set; } = new List<Posts>();    // 指向該分類下的文章集合
    }
}
