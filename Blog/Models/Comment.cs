using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "內容不能為空")]
        public string Content { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // 關聯到文章
        public int PostId { get; set; }
        public Posts Post { get; set; }

        // 關聯到使用者 (Identity)
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
