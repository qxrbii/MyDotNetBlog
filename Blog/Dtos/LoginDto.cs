using System.ComponentModel.DataAnnotations;

namespace Blog.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "請輸入帳號")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
