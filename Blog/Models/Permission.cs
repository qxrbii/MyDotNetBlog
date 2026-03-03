namespace Blog.Models
{
    public class Permission
    {
        public int Id { get; set; }   // 主鍵
        public int Role { get; set; }   // 角色（0: 一般使用者, 1: 管理員）
        public string Resource { get; set; } = string.Empty;    // 資源名稱，例如 "Post", "Category", "User"
        public string Action { get; set; } = string.Empty;  // 動作名稱，例如 "Create", "Read", "Update", "Delete"
    }
}
