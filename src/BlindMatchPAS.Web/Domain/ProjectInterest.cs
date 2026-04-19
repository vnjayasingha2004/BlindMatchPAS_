using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlindMatchPAS.Web.Models;

namespace BlindMatchPAS.Web.Models.Domain
{
    public class ProjectInterest
    {
        public int Id { get; set; }

        [Required]
        public int ProjectProposalId { get; set; }

        [ForeignKey(nameof(ProjectProposalId))]
        public ProjectProposal? ProjectProposal { get; set; }

        [Required]
        public string SupervisorUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(SupervisorUserId))]
        public ApplicationUser? SupervisorUser { get; set; }

        public bool IsConfirmed { get; set; } = false;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAtUtc { get; set; }
    }
}