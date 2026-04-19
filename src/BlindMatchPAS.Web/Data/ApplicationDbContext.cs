using BlindMatchPAS.Web.Models;
using BlindMatchPAS.Web.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProjectProposal> ProjectProposals => Set<ProjectProposal>();
        public DbSet<ProjectInterest> ProjectInterests => Set<ProjectInterest>();
        public DbSet<ProjectAttachment> ProjectAttachments => Set<ProjectAttachment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProjectProposal>()
                .HasOne(p => p.StudentUser)
                .WithMany()
                .HasForeignKey(p => p.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProjectProposal>()
                .HasOne(p => p.MatchedSupervisor)
                .WithMany()
                .HasForeignKey(p => p.MatchedSupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProjectInterest>()
                .HasOne(i => i.ProjectProposal)
                .WithMany()
                .HasForeignKey(i => i.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectInterest>()
                .HasOne(i => i.SupervisorUser)
                .WithMany()
                .HasForeignKey(i => i.SupervisorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProjectAttachment>()
                .HasOne(a => a.ProjectProposal)
                .WithMany(p => p.Attachments)
                .HasForeignKey(a => a.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}