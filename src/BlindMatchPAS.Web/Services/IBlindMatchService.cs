using BlindMatchPAS.Web.Models.Domain;

namespace BlindMatchPAS.Web.Services
{
    public interface IBlindMatchService
    {
        Task<(bool Success, string Message)> ExpressInterestAsync(int proposalId, string supervisorUserId, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> ConfirmMatchAsync(int interestId, string supervisorUserId, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> ReassignMatchAsync(int proposalId, string supervisorUserId, CancellationToken cancellationToken = default);
    }
}
