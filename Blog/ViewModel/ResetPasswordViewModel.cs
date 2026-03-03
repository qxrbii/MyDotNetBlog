using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入新密碼")]
        [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認新密碼")]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不一致。")]
        public string ConfirmPassword { get; set; }
    }
}