using System.Text.Json;

namespace BlindMatchPAS.Web.Services
{
    public class ResearchAreaCatalogService : IResearchAreaCatalogService
    {
        private static readonly string[] DefaultAreas =
        [
            "Artificial Intelligence",
            "Web Development",
            "Cybersecurity",
            "Cloud Computing",
            "Data Science",
            "Software Engineering",
            "Mobile Computing"
        ];

        private static readonly SemaphoreSlim SyncLock = new(1, 1);
        private readonly IWebHostEnvironment _environment;
        private readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = true };

        public ResearchAreaCatalogService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<List<string>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            await EnsureSeedFileAsync(cancellationToken);
            var path = GetFilePath();
            await using var stream = File.OpenRead(path);
            var areas = await JsonSerializer.DeserializeAsync<List<string>>(stream, cancellationToken: cancellationToken) ?? [];
            return areas
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(a => a)
                .ToList();
        }

        public async Task<(bool Success, string Message)> AddAsync(string areaName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(areaName))
            {
                return (false, "Research area name is required.");
            }

            var normalized = areaName.Trim();
            await SyncLock.WaitAsync(cancellationToken);
            try
            {
                var areas = await GetAllAsync(cancellationToken);
                if (areas.Any(a => string.Equals(a, normalized, StringComparison.OrdinalIgnoreCase)))
                {
                    return (false, "That research area already exists.");
                }

                areas.Add(normalized);
                areas = areas.OrderBy(a => a).ToList();
                await WriteAsync(areas, cancellationToken);
                return (true, "Research area added successfully.");
            }
            finally
            {
                SyncLock.Release();
            }
        }

        public async Task<(bool Success, string Message)> RemoveAsync(string areaName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(areaName))
            {
                return (false, "Research area name is required.");
            }

            await SyncLock.WaitAsync(cancellationToken);
            try
            {
                var areas = await GetAllAsync(cancellationToken);
                var removed = areas.RemoveAll(a => string.Equals(a, areaName.Trim(), StringComparison.OrdinalIgnoreCase));
                if (removed == 0)
                {
                    return (false, "Research area not found.");
                }

                await WriteAsync(areas, cancellationToken);
                return (true, "Research area removed successfully.");
            }
            finally
            {
                SyncLock.Release();
            }
        }

        private async Task EnsureSeedFileAsync(CancellationToken cancellationToken)
        {
            var path = GetFilePath();
            var directory = Path.GetDirectoryName(path)!;
            Directory.CreateDirectory(directory);

            if (File.Exists(path))
            {
                return;
            }

            await WriteAsync(DefaultAreas.ToList(), cancellationToken);
        }

        private async Task WriteAsync(List<string> areas, CancellationToken cancellationToken)
        {
            var path = GetFilePath();
            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, areas, _serializerOptions, cancellationToken);
        }

        private string GetFilePath()
        {
            return Path.Combine(_environment.ContentRootPath, "App_Data", "research-areas.json");
        }
    }
}
