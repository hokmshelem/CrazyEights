using System.ComponentModel.DataAnnotations;

namespace CrazyEights.Dtos.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Player name must be at least {2}, and maximum {1} charachters")]
        [RegularExpression("^[a-zA-Z0-9_.-]*$", ErrorMessage = "Player name must containe only a-z A-Z 0-9 characters")]
        public string PlayerName { get; set; }
        [Required]
        [RegularExpression("^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$",
            ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Password must be at least {2}, and maximum {1} charachters")]
        public string Password { get; set; }
    }
}
