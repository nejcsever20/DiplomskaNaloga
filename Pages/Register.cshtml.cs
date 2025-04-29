using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using diploma.Data;
using diploma.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace diploma.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string? Ime { get; set; }

            [Required]
            public string? Priimek { get; set; }

            [Required]
            [EmailAddress]
            public string? Email { get; set; }

            [Required]
            public string? Geslo { get; set; }

            [Required]
            public long Pravica { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new User
            {
                Ime = Input.Ime,
                Priimek = Input.Priimek,
                Email = Input.Email,
                Geslo = Input.Geslo, // You should hash this in production!
                Pravica = Input.Pravica
            };

            _context.diplomska_Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("Login");
        }
    }
}
