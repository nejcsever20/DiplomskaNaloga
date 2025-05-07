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

        // Load data on page load
        public async Task<IActionResult> OnGetAsync()
        {
            await LoadData();
            return Page();
        }

        // Set times for the process
        public async Task<IActionResult> OnPostSetTimesAsync()
        {
            TempData["SuccessMessage"] = "Čas uspešno nastavljen.";
            await LoadData();
            return Page();
        }

        // Save new Izkladisceno entry
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

        // Delete Izkladisceno entry
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the current user is in the Izmenovodja role
            if (!await _userManager.IsInRoleAsync(currentUser, "Izmenovodja"))
            {
                return Forbid(); // If not an Izmenovodja, deny access
            }

            // Find the record to delete
            var record = await _context.Izkladisceno.FindAsync(id);
            if (record == null)
            {
                TempData["ErrorMessage"] = "Zapis ni bil najden.";
                return RedirectToPage(); // Redirect to the same page if the record isn't found
            }

            // Delete the record and save changes to the database
            _context.Izkladisceno.Remove(record);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Zapis uspešno izbrisan.";
            return RedirectToPage(); // Redirect to refresh the page after deletion
        }

        // Add a note for the process
        public async Task<IActionResult> OnPostAddNoteAsync()
        {
            TempData["SuccessMessage"] = "Opomba dodana.";
            return RedirectToPage();
        }

        // Load data for Izkladisceno list and Skladiscnik select list
        private async Task LoadData()
        {
            IzkladiscenoList = await _context.Izkladisceno.OrderByDescending(x => x.Datum).ToListAsync();
            var users = await _userManager.Users.Select(u => u.UserName).ToListAsync();
            SkladiscnikSelectList = new SelectList(users);
        }
    }
}
