namespace BlindMatchPAS.Web.Services
{
    public interface IResearchAreaCatalogService
    {
        Task<List<string>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> AddAsync(string areaName, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> RemoveAsync(string areaName, CancellationToken cancellationToken = default);
    }
}
