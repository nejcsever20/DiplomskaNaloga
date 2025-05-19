using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace diplomska.Pages
{
    public class EditAccountItemsModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IWebHostEnvironment _env;

        public EditAccountItemsModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        [BindProperty, Required]
        public string UserName { get; set; } = string.Empty;

        [BindProperty, Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string? Name { get; set; }

        [BindProperty]
        public string? Surname { get; set; }

        [BindProperty, DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [BindProperty]
        public IFormFile? ProfileImage { get; set; }

        public string? CurrentProfileImagePath { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            UserName = user.UserName!;
            Email = user.Email!;

            var claims = await _userManager.GetClaimsAsync(user);

            Name = claims.FirstOrDefault(c => c.Type == "Name")?.Value;
            Surname = claims.FirstOrDefault(c => c.Type == "Surname")?.Value;
            CurrentProfileImagePath = claims.FirstOrDefault(c => c.Type == "ProfileImagePath")?.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            if (user.UserName != UserName)
                user.UserName = UserName;

            if (user.Email != Email)
                user.Email = Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            var claims = await _userManager.GetClaimsAsync(user);

            async Task UpdateClaim(string claimType, string? claimValue)
            {
                var existingClaim = claims.FirstOrDefault(c => c.Type == claimType);
                if (existingClaim != null)
                    await _userManager.RemoveClaimAsync(user, existingClaim);

                if (!string.IsNullOrEmpty(claimValue))
                    await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
            }

            await UpdateClaim("Name", Name);
            await UpdateClaim("Surname", Surname);

            if (ProfileImage != null)
            {
                var fileName = $"{Guid.NewGuid()}_{ProfileImage.FileName}";
                var uploadPath = Path.Combine(_env.WebRootPath, "images", "profiles");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await ProfileImage.CopyToAsync(stream);

                await UpdateClaim("ProfileImagePath", $"/images/profiles/{fileName}");
            }

            if (!string.IsNullOrEmpty(NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, NewPassword);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }
    }
}
