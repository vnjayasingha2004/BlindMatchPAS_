namespace BlindMatchPAS.Web.Services
{
    public interface ISupervisorExpertiseService
    {
        Task<List<string>> GetPreferredAreasAsync(string supervisorUserId);
        Task<(bool Success, string Message)> SavePreferredAreasAsync(string supervisorUserId, IReadOnlyCollection<string> preferredAreas);
    }
}
