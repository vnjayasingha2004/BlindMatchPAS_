using BlindMatchPAS.Web.Models.Domain;
using Microsoft.AspNetCore.Http;

namespace BlindMatchPAS.Web.Services
{
    public class AttachmentStorageService : IAttachmentStorageService
    {
        private static readonly HashSet<string> AllowedExtensions = [".pdf", ".jpg", ".jpeg", ".png"];
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;
        private readonly IWebHostEnvironment _environment;

        public AttachmentStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<List<ProjectAttachment>> SaveProposalAttachmentsAsync(int proposalId, IReadOnlyCollection<IFormFile> files, CancellationToken cancellationToken = default)
        {
            var savedFiles = new List<ProjectAttachment>();
            if (files.Count == 0)
            {
                return savedFiles;
            }

            var uploadFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "proposals");
            Directory.CreateDirectory(uploadFolder);

            foreach (var file in files)
            {
                if (file is null || file.Length == 0)
                {
                    continue;
                }

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension) || file.Length > MaxFileSizeBytes)
                {
                    continue;
                }

                var storedFileName = $"{Guid.NewGuid():N}{extension}";
                var fullPath = Path.Combine(uploadFolder, storedFileName);

                await using var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                await file.CopyToAsync(stream, cancellationToken);

                savedFiles.Add(new ProjectAttachment
                {
                    ProjectProposalId = proposalId,
                    OriginalFileName = Path.GetFileName(file.FileName),
                    StoredFileName = storedFileName,
                    RelativePath = $"/uploads/proposals/{storedFileName}",
                    ContentType = file.ContentType,
                    FileSizeBytes = file.Length
                });
            }

            return savedFiles;
        }

        public string? ResolveStoredPath(ProjectAttachment attachment)
        {
            if (string.IsNullOrWhiteSpace(attachment.RelativePath))
            {
                return null;
            }

            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativePath = attachment.RelativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.GetFullPath(Path.Combine(webRoot, relativePath));
            var normalizedRoot = Path.GetFullPath(webRoot);

            if (!fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return fullPath;
        }
    }
}
