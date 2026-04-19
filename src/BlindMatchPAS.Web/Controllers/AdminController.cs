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
    [Authorize(Roles = Roles.ModuleLeader + "," + Roles.SystemAdmin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IResearchAreaCatalogService _researchAreaCatalogService;
        private readonly IBlindMatchService _blindMatchService;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IResearchAreaCatalogService researchAreaCatalogService,
            IBlindMatchService blindMatchService)
        {
            _context = context;
            _userManager = userManager;
            _researchAreaCatalogService = researchAreaCatalogService;
            _blindMatchService = blindMatchService;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            return View(await BuildDashboardModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddResearchArea(string areaName)
        {
            var result = await _researchAreaCatalogService.AddAsync(areaName);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveResearchArea(string areaName)
        {
            var result = await _researchAreaCatalogService.RemoveAsync(areaName);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser([Bind(Prefix = "CreateUserForm")] CreateUserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please enter valid user account details.";
                return RedirectToAction(nameof(Dashboard));
            }


           var validRoles = new[] { Roles.Student, Roles.Supervisor, Roles.ModuleLeader };
            if (!validRoles.Contains(model.Role))
            {
                TempData["ErrorMessage"] = "Please select a valid application role.";
                return RedirectToAction(nameof(Dashboard));
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (existingUser is not null)
            {
                TempData["ErrorMessage"] = "A user with this email already exists.";
                return RedirectToAction(nameof(Dashboard));
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName.Trim(),
                Email = model.Email.Trim(),
                UserName = model.Email.Trim(),
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                TempData["ErrorMessage"] = string.Join(" ", createResult.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Dashboard));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                TempData["ErrorMessage"] = string.Join(" ", roleResult.Errors.Select(e => e.Description));
                return RedirectToAction(nameof(Dashboard));
            }

            TempData["SuccessMessage"] = "User account created successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reassign(int proposalId, string supervisorUserId)
        {
            var result = await _blindMatchService.ReassignMatchAsync(proposalId, supervisorUserId);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Dashboard));
        }

        private async Task<AdminDashboardViewModel> BuildDashboardModelAsync()
        {
            var proposals = await _context.ProjectProposals
                .Include(p => p.StudentUser)
                .Include(p => p.MatchedSupervisor)
                .OrderByDescending(p => p.CreatedAtUtc)
                .ToListAsync();

            var supervisors = await _userManager.GetUsersInRoleAsync(Roles.Supervisor);
            var students = await _userManager.GetUsersInRoleAsync(Roles.Student);

            return new AdminDashboardViewModel
            {
                AllProposals = proposals,
                Supervisors = supervisors.OrderBy(u => u.FullName).ToList(),
                Students = students.OrderBy(u => u.FullName).ToList(),
                ResearchAreas = await _researchAreaCatalogService.GetAllAsync(),
                TotalProposals = proposals.Count,
                PendingCount = proposals.Count(p => p.Status == ProposalStatuses.Pending),
                UnderReviewCount = proposals.Count(p => p.Status == ProposalStatuses.UnderReview),
                MatchedCount = proposals.Count(p => p.Status == ProposalStatuses.Matched)
            };
        }
    }
}
