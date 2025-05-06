using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using diplomska.Services;

namespace diplomska.Pages.Admin
{
    public class SetSmtpPasswordModel : PageModel
    {
        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            SecureSettings.SmtpPassword = Password;
            Message = "SMTP password set successfully.";
            return Page();
        }
    }
}
