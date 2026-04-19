using BlindMatchPAS.Web.Models;
using BlindMatchPAS.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlindMatchPAS.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SettingsController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var model = new SettingsViewModel
            {
                FullName = user.FullName,
                ThemePreference = user.ThemePreference,
                CurrentAvatarPath = user.AvatarPath
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(SettingsViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                model.CurrentAvatarPath = user.AvatarPath;
                return View(model);
            }

            user.FullName = model.FullName;
            user.ThemePreference = model.ThemePreference;

            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var ext = Path.GetExtension(model.AvatarFile.FileName).ToLowerInvariant();

                if (allowed.Contains(ext))
                {
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var path = Path.Combine(folder, fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await model.AvatarFile.CopyToAsync(stream);

                    user.AvatarPath = $"/uploads/avatars/{fileName}";
                }
            }

            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = "Profile settings updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}