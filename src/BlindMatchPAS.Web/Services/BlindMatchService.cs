using BlindMatchPAS.Web.Data;
using BlindMatchPAS.Web.Models.Common;
using BlindMatchPAS.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Web.Services
{
    public class BlindMatchService : IBlindMatchService
    {
        private readonly ApplicationDbContext _context;

        public BlindMatchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> ExpressInterestAsync(int proposalId, string supervisorUserId, CancellationToken cancellationToken = default)
        {
            var proposal = await _context.ProjectProposals
                .FirstOrDefaultAsync(p => p.Id == proposalId, cancellationToken);

            if (proposal is null)
            {
                return (false, "Proposal not found.");
            }

            if (!ProposalStatuses.IsBlindReviewVisible(proposal.Status))
            {
                return (false, "This proposal is not available for blind review.");
            }

            var alreadyExists = await _context.ProjectInterests.AnyAsync(i =>
                i.ProjectProposalId == proposalId &&
                i.SupervisorUserId == supervisorUserId,
                cancellationToken);

            if (alreadyExists)
            {
                return (false, "You already expressed interest in this proposal.");
            }

            _context.ProjectInterests.Add(new ProjectInterest
            {
                ProjectProposalId = proposal.Id,
                SupervisorUserId = supervisorUserId,
                IsConfirmed = false
            });

            if (proposal.Status == ProposalStatuses.Pending)
            {
                proposal.Status = ProposalStatuses.UnderReview;
            }

            proposal.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            return (true, "Interest submitted successfully.");
        }

        public async Task<(bool Success, string Message)> ConfirmMatchAsync(int interestId, string supervisorUserId, CancellationToken cancellationToken = default)
        {
            var interest = await _context.ProjectInterests
                .Include(i => i.ProjectProposal)
                .FirstOrDefaultAsync(i => i.Id == interestId && i.SupervisorUserId == supervisorUserId, cancellationToken);

            if (interest is null || interest.ProjectProposal is null)
            {
                return (false, "Interest record not found.");
            }

            var proposal = interest.ProjectProposal;
            if (proposal.Status == ProposalStatuses.Matched)
            {
                return (false, "This proposal is already matched.");
            }

            interest.IsConfirmed = true;
            interest.ConfirmedAtUtc = DateTime.UtcNow;

            proposal.Status = ProposalStatuses.Matched;
            proposal.MatchedSupervisorId = supervisorUserId;
            proposal.MatchedAtUtc = DateTime.UtcNow;
            proposal.IdentityRevealedAtUtc = DateTime.UtcNow;
            proposal.UpdatedAtUtc = DateTime.UtcNow;

            var competingInterests = await _context.ProjectInterests
                .Where(i => i.ProjectProposalId == proposal.Id && i.Id != interest.Id)
                .ToListAsync(cancellationToken);

            _context.ProjectInterests.RemoveRange(competingInterests);
            await _context.SaveChangesAsync(cancellationToken);
            return (true, "Match confirmed. Identity revealed.");
        }

        public async Task<(bool Success, string Message)> ReassignMatchAsync(int proposalId, string supervisorUserId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(supervisorUserId))
            {
                return (false, "Please choose a supervisor.");
            }

            var proposal = await _context.ProjectProposals
                .FirstOrDefaultAsync(p => p.Id == proposalId, cancellationToken);

            if (proposal is null)
            {
                return (false, "Proposal not found.");
            }

            proposal.Status = ProposalStatuses.Matched;
            proposal.MatchedSupervisorId = supervisorUserId;
            proposal.MatchedAtUtc ??= DateTime.UtcNow;
            proposal.IdentityRevealedAtUtc ??= DateTime.UtcNow;
            proposal.UpdatedAtUtc = DateTime.UtcNow;

            var interests = await _context.ProjectInterests
                .Where(i => i.ProjectProposalId == proposalId)
                .ToListAsync(cancellationToken);

            foreach (var interest in interests)
            {
                interest.IsConfirmed = interest.SupervisorUserId == supervisorUserId;
                interest.ConfirmedAtUtc = interest.IsConfirmed ? DateTime.UtcNow : null;
            }

            if (!interests.Any(i => i.SupervisorUserId == supervisorUserId))
            {
                _context.ProjectInterests.Add(new ProjectInterest
                {
                    ProjectProposalId = proposalId,
                    SupervisorUserId = supervisorUserId,
                    IsConfirmed = true,
                    ConfirmedAtUtc = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return (true, "Project assignment updated successfully.");
        }
    }
}
