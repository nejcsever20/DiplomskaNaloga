using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using diplomska.Data;
using diplomska.Models;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [BindProperty(SupportsGet = true)] public long TransportId { get; set; }
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
        public List<Izkladisceno> IzkladiscenoList { get; set; } = new List<Izkladisceno>();

        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<int> ChartData { get; set; } = new List<int>();
        public List<long> TransportIds { get; set; } = new List<long>();

        public async Task<IActionResult> OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Page();

            // Get all users in "Skladiščnik" role
            var allUsers = await _userManager.Users.ToListAsync();
            var skladiscniki = new List<object>();

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Skladiščnik"))
                {
                    skladiscniki.Add(new { FullName = user.UserName });
                }
            }

            SkladiscnikSelectList = new SelectList(skladiscniki, "FullName", "FullName");

            // Load Izkladisceno entries for current TransportId
            IzkladiscenoList = await _context.Izkladisceno
                .Where(i => i.TransportId == TransportId)
                .Select(item => new Izkladisceno
                {
                    Id = item.Id,
                    Kolicina = item.Kolicina ?? 0,
                    Palete = item.Palete ?? 0,
                    Datum = item.Datum,
                    Skladiscnik = item.Skladiscnik,
                    TransportId = item.TransportId
                })
                .ToListAsync();

            // Get latest transport for default selection if not already provided
            if (TransportId == 0)
            {
                var transport = await _context.Transport.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
                if (transport != null)
                {
                    TransportId = transport.Id;
                    StTransporta = transport.StTransporta ?? 0;
                }
                else
                {
                    TransportId = 0;
                    StTransporta = 0;
                }
            }

            // Group Izkladisceno by Skladiscnik for chart
            var groupedData = await _context.Izkladisceno
                .GroupBy(i => i.Skladiscnik)
                .Select(g => new
                {
                    Skladiscnik = g.Key,
                    TotalKolicina = g.Sum(x => x.Kolicina ?? 0),
                    Transports = g
                        .Where(x => x.TransportId != null)
                        .Select(x => x.TransportId)
                        .Distinct()
                        .ToList()
                })
                .ToListAsync();

            ChartLabels = groupedData.Select(g => g.Skladiscnik).ToList();
            ChartData = groupedData.Select(g => g.TotalKolicina).ToList();
            TransportIds = groupedData
                .SelectMany(g => g.Transports)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            return Page();
        }

        // Save Izkladisceno entry
        public async Task<IActionResult> OnPostSaveData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var transport = await _context.Transport.FirstOrDefaultAsync(t => t.Id == TransportId);
            if (transport == null)
            {
                ModelState.AddModelError(string.Empty, "Izbran transport ni veljaven.");
                return Page();
            }

            var izkladisceno = new Izkladisceno
            {
                Kolicina = Kolicina ?? 0,
                Palete = Palete ?? 0,
                Skladiscnik = Skladiscnik ?? "Unknown",
                Datum = DateTime.Now,
                SkladiscnikId = userId,
                TransportId = transport.Id
            };

            _context.Izkladisceno.Add(izkladisceno);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { TransportId = transport.Id });
        }

        public async Task<IActionResult> OnPostAddNote()
        {
            var transport = await _context.Transport.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            if (transport == null)
            {
                TempData["ErrorMessage"] = "Ni najdenega aktivnega transporta za shranjevanje opombe.";
                return RedirectToPage();
            }

            // Uncomment and set Notes property if Transport model has Notes field
            // transport.Notes = Notes;

            _context.Transport.Update(transport);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Opomba uspešno shranjena.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete(long id)
        {
            var izkladisceno = await _context.Izkladisceno.FindAsync(id);

            if (izkladisceno == null || izkladisceno.TransportId != TransportId)
            {
                return NotFound();
            }

            var currentUser = User.Identity?.Name;
            if (izkladisceno.Skladiscnik != currentUser)
            {
                return Unauthorized();
            }

            _context.Izkladisceno.Remove(izkladisceno);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { TransportId = TransportId });
        }
    }
}
