using diplomska.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Authorize]
public class UploadAvatarModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;

    public UploadAvatarModel(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext, IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _environment = environment;
    }

    [BindProperty]
    [Required(ErrorMessage = "Please select an image file.")]
    public IFormFile AvatarFile { get; set; } = default!;

    public string? Message { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Message = "Please select a valid image file.";
            return Page();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var ext = Path.GetExtension(AvatarFile.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
        {
            Message = "Unsupported file format. Please upload a JPG or PNG image.";
            return Page();
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Filename uniquely identifies the user avatar
        var fileName = $"{user.Id}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await AvatarFile.CopyToAsync(stream);
        }

        // Get or create the UserProfile record
        var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);

        if (profile == null)
        {
            profile = new UserProfile
            {
                UserId = user.Id,
                AvatarPath = $"/uploads/avatars/{fileName}"
            };
            _dbContext.UserProfiles.Add(profile);
        }
        else
        {
            profile.AvatarPath = $"/uploads/avatars/{fileName}";
            _dbContext.UserProfiles.Update(profile);
        }

        await _dbContext.SaveChangesAsync();

        Message = "Avatar uploaded successfully!";
        return Page();
    }
}
