using BlindMatchPAS.Web.Models.Domain;

namespace BlindMatchPAS.Web.Models.ViewModels
{
    public class SupervisorDashboardViewModel
    {
        public string? SelectedResearchArea { get; set; }
        public List<string> AvailableResearchAreas { get; set; } = new();
        public List<string> PreferredResearchAreas { get; set; } = new();
        public List<ProjectProposal> Proposals { get; set; } = new();
        public List<ProjectInterest> MyInterests { get; set; } = new();
    }
}
