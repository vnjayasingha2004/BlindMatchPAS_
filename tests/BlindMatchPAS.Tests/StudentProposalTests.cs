using BlindMatchPAS.Web.Data;
using BlindMatchPAS.Web.Models.Common;
using BlindMatchPAS.Web.Models.Domain;
using BlindMatchPAS.Web.Models.ViewModels;
using BlindMatchPAS.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Tests
{
    public class StudentProposalTests
    {
        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task WithdrawAsync_Sets_Status_To_Withdrawn()
        {
            await using var context = CreateDbContext();
            context.ProjectProposals.Add(new ProjectProposal
            {
                Title = "API Gateway",
                Abstract = "Gateway project.",
                TechnicalStack = ".NET",
                ResearchArea = "Cloud Computing",
                Status = ProposalStatuses.Pending,
                StudentUserId = "student-1"
            });
            await context.SaveChangesAsync();

            var service = new ProposalManagementService(context, new FakeAttachmentStorageService());
            var result = await service.WithdrawAsync("student-1", context.ProjectProposals.Single().Id);

            Assert.True(result.Success);
            Assert.Equal(ProposalStatuses.Withdrawn, context.ProjectProposals.Single().Status);
        }

        [Fact]
        public async Task UpdateAsync_Rejects_Matched_Proposal()
        {
            await using var context = CreateDbContext();
            context.ProjectProposals.Add(new ProjectProposal
            {
                Title = "Vision App",
                Abstract = "Vision project.",
                TechnicalStack = "Python",
                ResearchArea = "Artificial Intelligence",
                Status = ProposalStatuses.Matched,
                StudentUserId = "student-1"
            });
            await context.SaveChangesAsync();

            var service = new ProposalManagementService(context, new FakeAttachmentStorageService());
            var result = await service.UpdateAsync("student-1", new StudentProposalEditViewModel
            {
                Id = context.ProjectProposals.Single().Id,
                Title = "Updated Vision App",
                TechnicalStack = "Python",
                ResearchArea = "Artificial Intelligence",
                Abstract = "Updated"
            });

            Assert.False(result.Success);
            Assert.Contains("pending or under-review", result.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
