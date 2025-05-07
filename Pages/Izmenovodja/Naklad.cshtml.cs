using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace diplomska.Pages.Izmenovodja
{
    [Authorize]
    public class NakladModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NakladModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty] public int Rampa1 { get; set; }
        [BindProperty] public int Rampa2 { get; set; }
        [BindProperty] public string CarinskaVrvicva { get; set; }
        [BindProperty] public string UstreznostVozilca { get; set; }
        [BindProperty] public DateTime? ZacetekNaklada { get; set; }
        [BindProperty] public DateTime? KonecNaklada { get; set; }
        [BindProperty] public DateTime? ZavrnilZacetek { get; set; }
        [BindProperty] public int Kolicina { get; set; }
        [BindProperty] public int Palete { get; set; }
        [BindProperty] public string Skladiscnik { get; set; }
        [BindProperty] public string Notes { get; set; }

        public string StTransporta { get; set; } = "12345"; // Placeholder
        public List<Izkladisceno> IzkladiscenoList { get; set; } = new();
        public SelectList SkladiscnikSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostSetTimesAsync()
        {
            TempData["SuccessMessage"] = "Čas uspešno nastavljen.";
            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveDataAsync()
        {
            var item = new Izkladisceno
            {
                Kolicina = Kolicina,
                Palete = Palete,
                Skladiscnik = Skladiscnik,
                Datum = DateTime.Now
            };

            _context.Izkladisceno.Add(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Podatek uspešno dodan.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Izmenovodja"))
            {
                return Forbid();
            }

            var record = await _context.Izkladisceno.FindAsync(id);
            if (record == null)
            {
                TempData["ErrorMessage"] = "Zapis ni bil najden.";
                return RedirectToPage();
            }

            _context.Izkladisceno.Remove(record);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Zapis uspešno izbrisan.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddNoteAsync()
        {
            TempData["SuccessMessage"] = "Opomba dodana.";
            return RedirectToPage();
        }

        private async Task LoadData()
        {
            IzkladiscenoList = await _context.Izkladisceno.OrderByDescending(x => x.Datum).ToListAsync();
            var users = await _userManager.Users.Select(u => u.UserName).ToListAsync();
            SkladiscnikSelectList = new SelectList(users);
        }
    }
}
