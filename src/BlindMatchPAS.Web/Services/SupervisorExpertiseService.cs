using BlindMatchPAS.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BlindMatchPAS.Web.Services
{
    public class SupervisorExpertiseService : ISupervisorExpertiseService
    {
        public const string ClaimType = "BlindMatch.PreferredResearchArea";
        private readonly UserManager<ApplicationUser> _userManager;

        public SupervisorExpertiseService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<string>> GetPreferredAreasAsync(string supervisorUserId)
        {
            var user = await _userManager.FindByIdAsync(supervisorUserId);
            if (user is null)
            {
                return [];
            }

            var claims = await _userManager.GetClaimsAsync(user);
            return claims
                .Where(c => c.Type == ClaimType)
                .Select(c => c.Value)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(v => v)
                .ToList();
        }

        public async Task<(bool Success, string Message)> SavePreferredAreasAsync(string supervisorUserId, IReadOnlyCollection<string> preferredAreas)
        {
            var user = await _userManager.FindByIdAsync(supervisorUserId);
            if (user is null)
            {
                return (false, "Supervisor account not found.");
            }

            var currentClaims = await _userManager.GetClaimsAsync(user);
            var areaClaims = currentClaims.Where(c => c.Type == ClaimType).ToList();

            if (areaClaims.Count > 0)
            {
                var removeResult = await _userManager.RemoveClaimsAsync(user, areaClaims);
                if (!removeResult.Succeeded)
                {
                    return (false, "Unable to clear existing expertise selections.");
                }
            }

            var distinctAreas = preferredAreas
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .Select(a => a.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(a => new Claim(ClaimType, a))
                .ToList();

            if (distinctAreas.Count > 0)
            {
                var addResult = await _userManager.AddClaimsAsync(user, distinctAreas);
                if (!addResult.Succeeded)
                {
                    return (false, "Unable to save selected expertise areas.");
                }
            }

            return (true, "Expertise preferences updated successfully.");
        }
    }
}
