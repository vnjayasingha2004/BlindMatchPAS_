using BlindMatchPAS.Web.Models.Domain;
using BlindMatchPAS.Web.Services;

namespace BlindMatchPAS.Tests;

public class UnitTest1
{
    [Fact]
    public async Task FakeAttachmentStorageService_Returns_No_Attachments_When_Empty()
    {
        var service = new FakeAttachmentStorageService();
        var results = await service.SaveProposalAttachmentsAsync(1, Array.Empty<Microsoft.AspNetCore.Http.IFormFile>());
        Assert.Empty(results);
    }
}

internal sealed class FakeAttachmentStorageService : IAttachmentStorageService
{
    public Task<List<ProjectAttachment>> SaveProposalAttachmentsAsync(int proposalId, IReadOnlyCollection<Microsoft.AspNetCore.Http.IFormFile> files, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<ProjectAttachment>());
    }

    public string? ResolveStoredPath(ProjectAttachment attachment)
    {
        return null;
    }
}
