using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlindMatchPAS.Web.Models;

namespace BlindMatchPAS.Web.Models.Domain
{
    public class ProjectProposal
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Abstract { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string TechnicalStack { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ResearchArea { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        [Required]
        public string StudentUserId { get; set; } = string.Empty;

        public ICollection<ProjectAttachment> Attachments { get; set; } = new List<ProjectAttachment>();

        [ForeignKey(nameof(StudentUserId))]
        public ApplicationUser? StudentUser { get; set; }

        public string? MatchedSupervisorId { get; set; }

        [ForeignKey(nameof(MatchedSupervisorId))]
        public ApplicationUser? MatchedSupervisor { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public DateTime? MatchedAtUtc { get; set; }
        public DateTime? IdentityRevealedAtUtc { get; set; }
    }
}