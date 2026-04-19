using BlindMatchPAS.Web.Models.ViewModels;

namespace BlindMatchPAS.Web.Services
{
    public interface IProposalManagementService
    {
        Task<(bool Success, string Message)> CreateAsync(string studentUserId, StudentProposalCreateViewModel model, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> UpdateAsync(string studentUserId, StudentProposalEditViewModel model, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> WithdrawAsync(string studentUserId, int proposalId, CancellationToken cancellationToken = default);
    }
}
