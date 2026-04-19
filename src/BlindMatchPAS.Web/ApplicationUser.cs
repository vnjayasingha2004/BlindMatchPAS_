using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(120)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [StringLength(300)]
        public string? AvatarPath { get; set; }

        [StringLength(20)]
        public string ThemePreference { get; set; } = "light";
    }
}