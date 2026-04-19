using BlindMatchPAS.Web.Authorization;
using BlindMatchPAS.Web.Data;
using BlindMatchPAS.Web.Models;
using BlindMatchPAS.Web.Models.ViewModels;
using BlindMatchPAS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Web.Controllers
{
    [Authorize(Roles = Roles.Student)]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProposalManagementService _proposalManagementService;
        private readonly IResearchAreaCatalogService _researchAreaCatalogService;

        public StudentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IProposalManagementService proposalManagementService,
            IResearchAreaCatalogService researchAreaCatalogService)
        {
            _context = context;
            _userManager = userManager;
            _proposalManagementService = proposalManagementService;
            _researchAreaCatalogService = researchAreaCatalogService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            return View(await BuildDashboardModelAsync(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dashboard(StudentDashboardViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                var invalidModel = await BuildDashboardModelAsync(userId);
                invalidModel.NewProposal = model.NewProposal;
                return View(invalidModel);
            }

            var result = await _proposalManagementService.CreateAsync(userId, model.NewProposal);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var proposal = await _context.ProjectProposals
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentUserId == userId);

            if (proposal is null)
            {
                return NotFound();
            }

            var model = new StudentProposalEditViewModel
            {
                Id = proposal.Id,
                Title = proposal.Title,
                TechnicalStack = proposal.TechnicalStack,
                ResearchArea = proposal.ResearchArea,
                Abstract = proposal.Abstract,
                AvailableResearchAreas = await _researchAreaCatalogService.GetAllAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentProposalEditViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            model.AvailableResearchAreas = await _researchAreaCatalogService.GetAllAsync();
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _proposalManagementService.UpdateAsync(userId, model);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Dashboard));
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Withdraw(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var result = await _proposalManagementService.WithdrawAsync(userId, id);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        private async Task<StudentDashboardViewModel> BuildDashboardModelAsync(string userId)
        {
            var proposals = await _context.ProjectProposals
                .Include(p => p.Attachments)
                .Include(p => p.MatchedSupervisor)
                .Where(p => p.StudentUserId == userId)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync();

            return new StudentDashboardViewModel
            {
                MyProposals = proposals,
                AvailableResearchAreas = await _researchAreaCatalogService.GetAllAsync()
            };
        }
    }
}
