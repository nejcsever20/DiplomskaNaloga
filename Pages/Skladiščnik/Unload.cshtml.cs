using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using diplomska.Data;
using diplomska.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace diplomska.Pages.Skladiščnik
{
    public class UnloadModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UnloadModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty] public int? Kolicina { get; set; }
        [BindProperty] public int? Palete { get; set; }
        [BindProperty] public string Skladiscnik { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToPage("/Error/Unauthorized");
            }

            return Page();
        }

        public IActionResult OnPostUnload()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var izkladisceno = new Izkladisceno
            {
                Kolicina = Kolicina ?? 0,
                Palete = Palete ?? 0,
                Skladiscnik = Skladiscnik,
                Datum = DateTime.Now,
                SkladiscnikId = userId
            };

            _context.Izkladisceno.Add(izkladisceno);
            _context.SaveChanges();

            return RedirectToPage("/Skladiščnik/Naklad");
        }
    }
}
