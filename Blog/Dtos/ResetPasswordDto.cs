using System.ComponentModel.DataAnnotations;

namespace Blog.Dtos
{
    public class ResetPasswordDto
    {
        public int UserId { get; set; }
        [Required, MinLength(6)]
        public string NewPassword { get; set; }
    }
}
