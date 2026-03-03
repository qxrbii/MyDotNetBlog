using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class PostImage
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty; // 圖片路徑

        [MaxLength(200)]
        public string? Caption { get; set; } // (選用) 圖片說明/Alt文字

        public int SortOrder { get; set; } // (選用) 用來決定圖片顯示順序

        // --- 外鍵關聯設定 ---
        public int PostId { get; set; } // 外鍵：這張圖屬於哪篇文章

        [ForeignKey("PostId")]
        public Posts? Post { get; set; } // 導航屬性

    }
}
