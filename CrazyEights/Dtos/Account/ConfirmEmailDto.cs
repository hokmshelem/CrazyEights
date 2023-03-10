using System.ComponentModel.DataAnnotations;

namespace CrazyEights.Dtos.Account
{
    public class ConfirmEmailDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
