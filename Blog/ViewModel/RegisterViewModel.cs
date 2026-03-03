using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel
{
    public class RegisterViewModel
    {

        private string _username;
        private string _email;

        [Required(ErrorMessage = "請輸入帳號")]
        [Display(Name = "帳號")]
        public string UserName 
        { 
            get => _username; 
            set => _username = value?.Trim();
        }

        [Required(ErrorMessage = "Email 是必填的")]
        [EmailAddress(ErrorMessage = "請輸入有效的 Email 格式")]
        [Display(Name = "電子郵件")]
        public string Email
        {
            get => _email;
            set => _email = value?.Trim();
        }

        [Required(ErrorMessage = "密碼是必填的")]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不相符")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "請輸入驗證碼")]
        public string CaptchaCode { get; set; }
    }
}
