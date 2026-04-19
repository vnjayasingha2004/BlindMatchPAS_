using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Web.Models.ViewModels
{
    public class SettingsViewModel
    {
        [Required]
        [StringLength(120)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Theme")]
        public string ThemePreference { get; set; } = "light";

        [Display(Name = "Profile Picture")]
        public IFormFile? AvatarFile { get; set; }

        public string? CurrentAvatarPath { get; set; }
    }
}