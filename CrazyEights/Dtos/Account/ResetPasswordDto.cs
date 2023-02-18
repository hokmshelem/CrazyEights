using System.ComponentModel.DataAnnotations;

namespace CrazyEights.Dtos.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} charachters")]
        public string NewPassword { get; set; }
    }
}
