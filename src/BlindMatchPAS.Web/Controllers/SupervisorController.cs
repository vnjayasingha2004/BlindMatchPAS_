using BlindMatchPAS.Web.Authorization;
using BlindMatchPAS.Web.Data;
using BlindMatchPAS.Web.Models;
using BlindMatchPAS.Web.Models.Common;
using BlindMatchPAS.Web.Models.ViewModels;
using BlindMatchPAS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Web.Controllers
{
    [Authorize(Roles = Roles.Supervisor)]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBlindMatchService _blindMatchService;
        private readonly IResearchAreaCatalogService _researchAreaCatalogService;
        private readonly ISupervisorExpertiseService _supervisorExpertiseService;

        public SupervisorController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IBlindMatchService blindMatchService,
            IResearchAreaCatalogService researchAreaCatalogService,
            ISupervisorExpertiseService supervisorExpertiseService)
        {
            _context = context;
            _userManager = userManager;
            _blindMatchService = blindMatchService;
            _researchAreaCatalogService = researchAreaCatalogService;
            _supervisorExpertiseService = supervisorExpertiseService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(string? researchArea)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            return View(await BuildDashboardModelAsync(userId, researchArea));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveExpertise(List<string> preferredResearchAreas)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var result = await _supervisorExpertiseService.SavePreferredAreasAsync(userId, preferredResearchAreas);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExpressInterest(int proposalId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var result = await _blindMatchService.ExpressInterestAsync(proposalId, userId);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMatch(int interestId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var result = await _blindMatchService.ConfirmMatchAsync(interestId, userId);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        private async Task<SupervisorDashboardViewModel> BuildDashboardModelAsync(string userId, string? researchArea)
        {
            var availableAreas = await _researchAreaCatalogService.GetAllAsync();
            var preferredAreas = await _supervisorExpertiseService.GetPreferredAreasAsync(userId);
            var selectedArea = string.IsNullOrWhiteSpace(researchArea) ? null : researchArea.Trim();

            var query = _context.ProjectProposals
                .AsNoTracking()
                .Include(p => p.Attachments)
                .Where(p => p.Status == ProposalStatuses.Pending || p.Status == ProposalStatuses.UnderReview);

            if (!string.IsNullOrWhiteSpace(selectedArea))
            {
                query = query.Where(p => p.ResearchArea == selectedArea);
            }
            else if (preferredAreas.Count > 0)
            {
                query = query.Where(p => preferredAreas.Contains(p.ResearchArea));
            }

            var proposals = await query
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync();

            var myInterests = await _context.ProjectInterests
                .Include(i => i.ProjectProposal)
                    .ThenInclude(p => p!.Attachments)
                .Include(i => i.ProjectProposal)
                    .ThenInclude(p => p!.StudentUser)
                .Where(i => i.SupervisorUserId == userId)
                .OrderByDescending(i => i.CreatedAtUtc)
                .ToListAsync();

            return new SupervisorDashboardViewModel
            {
                SelectedResearchArea = selectedArea,
                AvailableResearchAreas = availableAreas,
                PreferredResearchAreas = preferredAreas,
                Proposals = proposals,
                MyInterests = myInterests
            };
        }
    }
}
