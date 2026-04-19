using BlindMatchPAS.Web.Models.Domain;
using Microsoft.AspNetCore.Http;

namespace BlindMatchPAS.Web.Services
{
    public interface IAttachmentStorageService
    {
        Task<List<ProjectAttachment>> SaveProposalAttachmentsAsync(int proposalId, IReadOnlyCollection<IFormFile> files, CancellationToken cancellationToken = default);
        string? ResolveStoredPath(ProjectAttachment attachment);
    }
}
