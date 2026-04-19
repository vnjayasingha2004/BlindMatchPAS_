using BlindMatchPAS.Web.Models.Domain;

namespace BlindMatchPAS.Web.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public StudentProposalCreateViewModel NewProposal { get; set; } = new();
        public List<ProjectProposal> MyProposals { get; set; } = new();
        public List<string> AvailableResearchAreas { get; set; } = new();
    }
}
