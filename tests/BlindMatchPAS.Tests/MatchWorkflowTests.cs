using BlindMatchPAS.Web.Data;
using BlindMatchPAS.Web.Models.Common;
using BlindMatchPAS.Web.Models.Domain;
using BlindMatchPAS.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Tests
{
    public class MatchWorkflowTests
    {
        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ExpressInterest_Changes_Pending_To_UnderReview()
        {
            await using var context = CreateDbContext();
            context.ProjectProposals.Add(new ProjectProposal
            {
                Title = "Cloud Security Platform",
                Abstract = "Secure cloud review platform.",
                TechnicalStack = ".NET, Azure, SQL Server",
                ResearchArea = "Cloud Computing",
                Status = ProposalStatuses.Pending,
                StudentUserId = "student-1"
            });
            await context.SaveChangesAsync();

            var service = new BlindMatchService(context);
            var result = await service.ExpressInterestAsync(context.ProjectProposals.Single().Id, "supervisor-1");

            Assert.True(result.Success);
            Assert.Equal(ProposalStatuses.UnderReview, context.ProjectProposals.Single().Status);
            Assert.Single(context.ProjectInterests);
        }

        [Fact]
        public async Task ConfirmMatch_Reveals_Identity_And_Stores_Supervisor()
        {
            await using var context = CreateDbContext();
            var proposal = new ProjectProposal
            {
                Title = "Blind Match Workflow",
                Abstract = "Review workflow.",
                TechnicalStack = ".NET, SQL Server",
                ResearchArea = "Software Engineering",
                Status = ProposalStatuses.UnderReview,
                StudentUserId = "student-1"
            };
            context.ProjectProposals.Add(proposal);
            await context.SaveChangesAsync();

            var interest = new ProjectInterest
            {
                ProjectProposalId = proposal.Id,
                SupervisorUserId = "supervisor-1"
            };
            context.ProjectInterests.Add(interest);
            await context.SaveChangesAsync();

            var service = new BlindMatchService(context);
            var result = await service.ConfirmMatchAsync(interest.Id, "supervisor-1");

            var savedProposal = await context.ProjectProposals.SingleAsync();
            var savedInterest = await context.ProjectInterests.SingleAsync();

            Assert.True(result.Success);
            Assert.True(savedInterest.IsConfirmed);
            Assert.Equal(ProposalStatuses.Matched, savedProposal.Status);
            Assert.Equal("supervisor-1", savedProposal.MatchedSupervisorId);
            Assert.NotNull(savedProposal.IdentityRevealedAtUtc);
        }
    }
}
