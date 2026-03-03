using Microsoft.AspNetCore.Identity; // 1. 務必引用這個命名空間

namespace Blog.Models
{
    // 2. 關鍵在冒號後面的 ApplicationUser，這代表繼承
    public class ApplicationUser : IdentityUser
    {
        // 您的自定義欄位
        public bool IsMuted { get; set; } = false;
    }
}