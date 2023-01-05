using Microsoft.AspNetCore.Identity;
using System;

namespace CrazyEights.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string PlayerName { get; set; }
        public DateTime AccountCreated { get; set; } = DateTime.UtcNow;
        public int Score { get; set; } = 0;
        public string AboutMe { get; set; }
    }
}
