namespace Blog.Dtos
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string RoleName => Role == 1 ? "管理員" : "一般使用者"; // 邏輯寫在 DTO 減輕 View 負擔
        public int Role { get; set; }
    }
}
