using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Web.Models.ViewModels
{
    public class StudentProposalEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Project Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        [Display(Name = "Technical Stack")]
        public string TechnicalStack { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Research Area")]
        public string ResearchArea { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Abstract")]
        public string Abstract { get; set; } = string.Empty;

        [Display(Name = "Add More Attachments")]
        public List<IFormFile> NewAttachments { get; set; } = new();

        public List<string> AvailableResearchAreas { get; set; } = new();
    }
}
