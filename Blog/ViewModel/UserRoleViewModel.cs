namespace Blog.ViewModel
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; } // 用來判斷是否擁有 Admin 權限
        public bool IsMuted { get; set; }
    }
}
