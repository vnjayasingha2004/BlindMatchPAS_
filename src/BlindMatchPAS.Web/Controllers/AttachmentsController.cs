using BlindMatchPAS.Web.Authorization;
using BlindMatchPAS.Web.Data;
using BlindMatchPAS.Web.Models;
using BlindMatchPAS.Web.Models.Common;
using BlindMatchPAS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Web.Controllers
{
    [Authorize]
    public class AttachmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAttachmentStorageService _attachmentStorageService;

        public AttachmentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAttachmentStorageService attachmentStorageService)
        {
            _context = context;
            _userManager = userManager;
            _attachmentStorageService = attachmentStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Download(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var attachment = await _context.ProjectAttachments
                .Include(a => a.ProjectProposal)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attachment?.ProjectProposal is null)
            {
                return NotFound();
            }

            var proposal = attachment.ProjectProposal;
            var isOwner = proposal.StudentUserId == userId;
            var isAdmin = User.IsInRole(Roles.ModuleLeader) || User.IsInRole(Roles.SystemAdmin);
            var isMatchedSupervisor = proposal.MatchedSupervisorId == userId;
            var isBlindReviewSupervisor = User.IsInRole(Roles.Supervisor) && (proposal.Status == ProposalStatuses.Pending || proposal.Status == ProposalStatuses.UnderReview || proposal.MatchedSupervisorId == userId);

            if (!isOwner && !isAdmin && !isMatchedSupervisor && !isBlindReviewSupervisor)
            {
                return Forbid();
            }

            var fullPath = _attachmentStorageService.ResolveStoredPath(attachment);
            if (string.IsNullOrWhiteSpace(fullPath) || !System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            var contentType = string.IsNullOrWhiteSpace(attachment.ContentType)
                ? "application/octet-stream"
                : attachment.ContentType;

            var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            return File(bytes, contentType, attachment.OriginalFileName);
        }
    }
}
