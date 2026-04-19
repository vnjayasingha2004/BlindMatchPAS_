using BlindMatchPAS.Web.Data;
using BlindMatchPAS.Web.Models.Common;
using BlindMatchPAS.Web.Models.Domain;
using BlindMatchPAS.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Web.Services
{
    public class ProposalManagementService : IProposalManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAttachmentStorageService _attachmentStorageService;

        public ProposalManagementService(ApplicationDbContext context, IAttachmentStorageService attachmentStorageService)
        {
            _context = context;
            _attachmentStorageService = attachmentStorageService;
        }

        public async Task<(bool Success, string Message)> CreateAsync(string studentUserId, StudentProposalCreateViewModel model, CancellationToken cancellationToken = default)
        {
            var proposal = new ProjectProposal
            {
                Title = model.Title.Trim(),
                TechnicalStack = model.TechnicalStack.Trim(),
                ResearchArea = model.ResearchArea.Trim(),
                Abstract = model.Abstract.Trim(),
                Status = ProposalStatuses.Pending,
                StudentUserId = studentUserId,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.ProjectProposals.Add(proposal);
            await _context.SaveChangesAsync(cancellationToken);

            var attachments = await _attachmentStorageService.SaveProposalAttachmentsAsync(proposal.Id, model.Attachments ?? [], cancellationToken);
            if (attachments.Count > 0)
            {
                _context.ProjectAttachments.AddRange(attachments);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return (true, "Proposal submitted successfully.");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(string studentUserId, StudentProposalEditViewModel model, CancellationToken cancellationToken = default)
        {
            var proposal = await _context.ProjectProposals
                .Include(p => p.Attachments)
                .FirstOrDefaultAsync(p => p.Id == model.Id && p.StudentUserId == studentUserId, cancellationToken);

            if (proposal is null)
            {
                return (false, "Proposal not found.");
            }

            if (!ProposalStatuses.IsOpenForStudentChanges(proposal.Status))
            {
                return (false, "Only pending or under-review proposals can be edited.");
            }

            proposal.Title = model.Title.Trim();
            proposal.TechnicalStack = model.TechnicalStack.Trim();
            proposal.ResearchArea = model.ResearchArea.Trim();
            proposal.Abstract = model.Abstract.Trim();
            proposal.UpdatedAtUtc = DateTime.UtcNow;

            var newAttachments = await _attachmentStorageService.SaveProposalAttachmentsAsync(proposal.Id, model.NewAttachments ?? [], cancellationToken);
            if (newAttachments.Count > 0)
            {
                _context.ProjectAttachments.AddRange(newAttachments);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return (true, "Proposal updated successfully.");
        }

        public async Task<(bool Success, string Message)> WithdrawAsync(string studentUserId, int proposalId, CancellationToken cancellationToken = default)
        {
            var proposal = await _context.ProjectProposals
                .FirstOrDefaultAsync(p => p.Id == proposalId && p.StudentUserId == studentUserId, cancellationToken);

            if (proposal is null)
            {
                return (false, "Proposal not found.");
            }

            if (!ProposalStatuses.IsOpenForStudentChanges(proposal.Status))
            {
                return (false, "Only pending or under-review proposals can be withdrawn.");
            }

            proposal.Status = ProposalStatuses.Withdrawn;
            proposal.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return (true, "Proposal withdrawn successfully.");
        }
    }
}
