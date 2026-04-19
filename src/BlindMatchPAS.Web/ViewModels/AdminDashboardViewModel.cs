using BlindMatchPAS.Web.Models;
using BlindMatchPAS.Web.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Web.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<ProjectProposal> AllProposals { get; set; } = new();
        public List<ApplicationUser> Supervisors { get; set; } = new();
        public List<ApplicationUser> Students { get; set; } = new();
        public List<string> ResearchAreas { get; set; } = new();
        public CreateUserFormViewModel CreateUserForm { get; set; } = new();

        public int TotalProposals { get; set; }
        public int PendingCount { get; set; }
        public int UnderReviewCount { get; set; }
        public int MatchedCount { get; set; }
    }

    public class CreateUserFormViewModel
    {
        [Required]
        [StringLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
