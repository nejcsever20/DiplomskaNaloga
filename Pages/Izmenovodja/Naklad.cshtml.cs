using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using diplomska.Data;
using diplomska.Models;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace diplomska.Pages.Izmenovodja
{
    public class NakladModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NakladModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty] public int? Kolicina { get; set; }
        [BindProperty] public int? Palete { get; set; }
        [BindProperty] public string Skladiscnik { get; set; }
        [BindProperty] public long TransportId { get; set; }
        [BindProperty] public long? StTransporta { get; set; }
        [BindProperty] public int Rampa1 { get; set; }
        [BindProperty] public int Rampa2 { get; set; }
        [BindProperty] public string CarinskaVrvicva { get; set; } = "ni prisotna";
        [BindProperty] public string UstreznostVozilca { get; set; } = "neustrezno";
        [BindProperty] public DateTime? ZacetekNaklada { get; set; }
        [BindProperty] public DateTime? KonecNaklada { get; set; }
        [BindProperty] public DateTime? ZavrnilZacetek { get; set; }
        [BindProperty] public string Notes { get; set; }

        public SelectList SkladiscnikSelectList { get; set; }
        public List<Izkladisceno> IzkladiscenoList { get; set; } = new List<Izkladisceno>(); // Initialize as empty list

        public async Task<IActionResult> OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Page();

            // get every single user
            var allUsers = await _userManager.Users.ToListAsync();

            // Get the list of users for Skladiscnik dropdown
            var skladiscniki = allUsers
                   .Where(u => _userManager.IsInRoleAsync(u, "Skladiščnik").Result) // This will filter users by role in memory
                   .Select(u => new { FullName = u.UserName })
                   .ToList();

            // Fill the Skladiscnik dropdown
            SkladiscnikSelectList = new SelectList(skladiscniki, "FullName", "FullName");

            // Get the list of already existing Izkladisceno entries
            IzkladiscenoList = await _context.Izkladisceno.ToListAsync();  // Ensure this is properly initialized

            // Simulate Transport and StTransporta (you may need to load these based on actual transport data)
            var transport = await _context.Transport.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            if (transport != null)
            {
                TransportId = transport.Id;  // Safely assign non-nullable long
                StTransporta = transport.StTransporta ?? 0;  // Use 0 as fallback if null
            }

            return Page();
        }

        // Handler to save the data
        public async Task<IActionResult> OnPostSaveData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var izkladisceno = new Izkladisceno
            {
                Kolicina = Kolicina ?? 0,
                Palete = Palete ?? 0,
                Skladiscnik = Skladiscnik ?? "Unknown",
                Datum = DateTime.Now,
                SkladiscnikId = userId
            };

            // Save to the database
            _context.Izkladisceno.Add(izkladisceno);
            await _context.SaveChangesAsync();

            // After saving, reload the list of Izkladisceno entries
            IzkladiscenoList = await _context.Izkladisceno.ToListAsync();  // Reload updated list

            // Redirect to the same page to show the updated list
            return RedirectToPage();
        }

        // Handler for adding notes
        public async Task<IActionResult> OnPostAddNote()
        {
            // Logic for adding a note goes here if necessary.
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteItem(int itemId)
        {
            var item = await _context.Izkladisceno.FindAsync(itemId);
            if (item != null)
            {
                _context.Izkladisceno.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}