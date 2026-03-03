using System.ComponentModel.DataAnnotations;

namespace Blog.ViewComponents
{
    public class ForgotAccountViewModel
    {
        [Required(ErrorMessage = "請輸入註冊時的電子郵件")]
        [EmailAddress(ErrorMessage = "電子郵件格式錯誤")]
        public string Email { get; set; }
    }
}
