using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using diploma.Data;
using diploma.Models;
namespace diploma.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        public LoginModel(AppDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Geslo { get; set; }
        public IActionResult OnPost()
        {
            var user = _context.diplomska_Users.FirstOrDefault(u => u.Email == Email);
            if (user != null)
            {
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, user.Geslo, Geslo);
                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("ime", user.Ime.ToString());
                    HttpContext.Session.SetString("priimek", user.Priimek.ToString());
                    HttpContext.Session.SetString("email", user.Email.ToString());
                    HttpContext.Session.SetString("vloga", user.Pravica.ToString());

                    return RedirectToPage("/Session");
                }
            }
            ModelState.AddModelError(string.Empty, "Napačen email ali geslo.");
            return Page();
        }
    }
}