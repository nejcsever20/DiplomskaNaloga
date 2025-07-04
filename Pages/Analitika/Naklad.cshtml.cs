using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using diplomska.Data;
using diplomska.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Analitika
{
    [Authorize(Roles = "Admin, Analitika")]
    public class NakladModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NakladModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Bind Properties for form input
        [BindProperty] public int? Kolicina { get; set; }
        [BindProperty] public int? Palete { get; set; }
        [BindProperty] public string Skladiscnik { get; set; }
        [BindProperty(SupportsGet = true)] public long TransportId { get; set; }
        [BindProperty] public long? StTransporta { get; set; }
        [BindProperty] public string CarinskaVrvicva { get; set; }
        [BindProperty] public DateTime? KonecNaklada { get; set; }
        [BindProperty] public int? Rampa1 { get; set; }
        [BindProperty] public int? Rampa2 { get; set; }
        [BindProperty] public string UstreznostVozilca { get; set; }
        [BindProperty] public DateTime? ZacetekNaklada { get; set; }
        [BindProperty] public DateTime? ZavrnilZacetek { get; set; }
        [BindProperty] public string Notes { get; set; }

        // Properties used in the Razor page
        public SelectList SkladiscnikSelectList { get; set; }
        public List<Izkladisceno> IzkladiscenoList { get; set; } = new();
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartData { get; set; } = new();
        public List<long> TransportIds { get; set; } = new();

        // Helper method to load skladiscnik dropdown
        private async Task LoadSkladiscnikSelectListAsync()
        {
            var skladiscniki = new List<string>();

            var allUsers = await _userManager.Users.ToListAsync();
            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Skladiščnik"))
                {
                    skladiscniki.Add(user.UserName);
                }
            }

            SkladiscnikSelectList = new SelectList(skladiscniki);
        }

        public async Task<IActionResult> OnGet(long? stTransporta, long? id)
        {
            await LoadSkladiscnikSelectListAsync();

            // If 'id' is used in URL, resolve it to stTransporta
            if (!stTransporta.HasValue && id.HasValue)
            {
                var transport = await _context.Transport.FirstOrDefaultAsync(t => t.Id == id.Value);
                if (transport != null)
                {
                    stTransporta = transport.StTransporta;
                    TransportId = transport.Id;
                }
            }

            // If stTransporta is provided, resolve it to TransportId
            if (stTransporta.HasValue)
            {
                var transport = await _context.Transport.FirstOrDefaultAsync(t => t.StTransporta == stTransporta.Value);
                if (transport != null)
                {
                    TransportId = transport.Id;
                    StTransporta = transport.StTransporta;
                }
            }

            // Fallback to most recent transport
            if (TransportId == 0)
            {
                var latestTransport = await _context.Transport.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
                if (latestTransport != null)
                {
                    TransportId = latestTransport.Id;
                    StTransporta = latestTransport.StTransporta;
                }
            }

            // Load related data
            IzkladiscenoList = await _context.Izkladisceno
                .Where(i => i.TransportId == TransportId)
                .ToListAsync();

            var groupedData = await _context.Izkladisceno
                .GroupBy(i => i.Skladiscnik)
                .Select(g => new
                {
                    Skladiscnik = g.Key,
                    TotalKolicina = g.Sum(x => x.Kolicina ?? 0),
                    Transports = g
                        .Where(x => x.TransportId != null)
                        .Select(x => x.TransportId.Value)
                        .Distinct()
                        .ToList()
                })
                .ToListAsync();

            ChartLabels = groupedData.Select(g => g.Skladiscnik).ToList();
            ChartData = groupedData.Select(g => g.TotalKolicina).ToList();
            TransportIds = groupedData
                .SelectMany(g => g.Transports)
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            await LoadSkladiscnikSelectListAsync();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = string.Join(" ", errors);
                return Page();
            }

            var transport = await _context.Transport.FirstOrDefaultAsync(t => t.Id == TransportId);
            if (transport == null)
            {
                TempData["ErrorMessage"] = "Transport ni bil najden.";
                return RedirectToPage(new { TransportId });
            }


            // Update Transport - use null-coalescing to avoid errors
            transport.NAVISZacetekSklada = ZacetekNaklada;
            transport.NAVISKonecSklada = KonecNaklada;
            transport.Rampa1 = Rampa1 ?? 0;
            transport.Rampa2 = Rampa2 ?? 0;
            transport.CarinskaVrvicva = CarinskaVrvicva;
            transport.UstreznostVozilca = UstreznostVozilca;
            transport.ZavrnilZacetek = ZavrnilZacetek;

            _context.Transport.Update(transport);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var izkladisceno = new Izkladisceno
            {
                Kolicina = Kolicina,
                Palete = Palete,
                Datum = DateTime.Now,
                Skladiscnik = Skladiscnik ?? User.Identity?.Name ?? "Unknown",
                SkladiscnikId = userId,
                TransportId = TransportId,
                Rampa1 = Rampa1 ?? 0,
                Rampa2 = Rampa2 ?? 0,
                CarinskaVrvicva = CarinskaVrvicva,
                UstreznostVozilca = UstreznostVozilca,
                ZacetekNaklada = ZacetekNaklada,
                KonecNaklada = KonecNaklada,
                ZavrnilZacetek = ZavrnilZacetek
            };

            _context.Izkladisceno.Add(izkladisceno);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Napaka pri shranjevanju podatkov: " + ex.Message;
                return Page();
            }

            TempData["SuccessMessage"] = "Podatki uspešno shranjeni.";
            return RedirectToPage(new { TransportId });
        }

        public async Task<IActionResult> OnPostSaveData()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            await LoadSkladiscnikSelectListAsync();

            var transport = await _context.Transport.FirstOrDefaultAsync(t => t.Id == TransportId);
            if (transport == null)
            {
                ModelState.AddModelError(string.Empty, "Izbran transport ni veljaven.");
                return Page();
            }

            var izkladisceno = new Izkladisceno
            {
                Kolicina = Kolicina,
                Palete = Palete,
                Datum = DateTime.Now,
                Skladiscnik = Skladiscnik ?? User.Identity?.Name ?? "Unknown",
                SkladiscnikId = userId,
                TransportId = TransportId,
                Rampa1 = Rampa1 ?? 0,
                Rampa2 = Rampa2 ?? 0,
                CarinskaVrvicva = CarinskaVrvicva,
                UstreznostVozilca = UstreznostVozilca,
                ZacetekNaklada = ZacetekNaklada,
                KonecNaklada = KonecNaklada,
                ZavrnilZacetek = ZavrnilZacetek
            };

            _context.Izkladisceno.Add(izkladisceno);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { TransportId = transport.Id });
        }

        public async Task<IActionResult> OnPostAddNote()
        {
            if (TransportId == 0)
            {
                TempData["ErrorMessage"] = "Ni izbran transport za shranjevanje opombe.";
                return RedirectToPage();
            }

            var transport = await _context.Transport.FirstOrDefaultAsync(t => t.Id == TransportId);
            if (transport == null)
            {
                TempData["ErrorMessage"] = "Transport ni bil najden.";
                return RedirectToPage(new { TransportId });
            }


            transport.Notes = Notes;

            _context.Transport.Update(transport);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Opomba uspešno shranjena.";
            return RedirectToPage(new { TransportId });
        }

        public async Task<IActionResult> OnPostDelete(long id)
        {
            var izkladisceno = await _context.Izkladisceno.FindAsync(id);

            if (izkladisceno == null || izkladisceno.TransportId != TransportId)
                return NotFound();

            var currentUser = User.Identity?.Name;
            if (izkladisceno.Skladiscnik != currentUser && !User.IsInRole("Admin"))
                // Allow Admins to delete any record; others only their own
                return Unauthorized();

            _context.Izkladisceno.Remove(izkladisceno);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { TransportId });
        }
    }
}
