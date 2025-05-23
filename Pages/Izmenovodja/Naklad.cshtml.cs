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
        public List<Izkladisceno> IzkladiscenoList { get; set; } = new List<Izkladisceno>();

        public async Task<IActionResult> OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Page();

            // Get all users from the database
            var allUsers = await _userManager.Users.ToListAsync();

            // Filter the users who are in the "Skladiščnik" role
            var skladiscniki = new List<object>();
            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Skladiščnik"))
                {
                    skladiscniki.Add(new { FullName = user.UserName });
                }
            }

            // Fill the Skladiscnik dropdown
            SkladiscnikSelectList = new SelectList(skladiscniki, "FullName", "FullName");

            // Get the list of already existing Izkladisceno entries
            IzkladiscenoList = await _context.Izkladisceno
                                              .Select(item => new Izkladisceno
                                              {
                                                  Kolicina = item.Kolicina ?? 0,   // Default to 0 if Kolicina is NULL
                                                  Palete = item.Palete ?? 0,       // Default to 0 if Palete is NULL
                                                  Datum = item.Datum, // Default to DateTime.MinValue if Datum is NULL
                                                  Skladiscnik = item.Skladiscnik,
                                              })
                                              .ToListAsync();

            // Get the latest transport data
            var transport = await _context.Transport.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            if (transport != null)
            {
                TransportId = transport.Id;  // Set the transport Id
                StTransporta = transport.StTransporta ?? 0;  // Set StTransporta to 0 if null
            }
            else
            {
                TransportId = 0;  // Handle the case where there are no transports
                StTransporta = 0;
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

            // Example: fetch a valid transport (you should replace this logic with actual selection or input)
            var transport = await _context.Transport.FirstOrDefaultAsync(); // adjust to your actual table name and logic
            if (transport == null)
            {
                ModelState.AddModelError(string.Empty, "No valid transport found. Please create one first.");
                return Page();
            }

            var izkladisceno = new Izkladisceno
            {
                Kolicina = Kolicina ?? 0,
                Palete = Palete ?? 0,
                Skladiscnik = Skladiscnik ?? "Unknown",
                Datum = DateTime.Now,
                SkladiscnikId = userId,
                TransportId = transport.Id // ✅ This is the required line
            };

            _context.Izkladisceno.Add(izkladisceno);
            await _context.SaveChangesAsync();

            IzkladiscenoList = await _context.Izkladisceno.ToListAsync();

            return RedirectToPage();
        }


        // Handler for adding notes
        public async Task<IActionResult> OnPostAddNote()
        {
            // Logic for adding a note goes here if necessary.
            return RedirectToPage();
        }

        // Handler for deleting an entry
        public async Task<IActionResult> OnPostDelete(long id)
        {
            var izkladisceno = await _context.Izkladisceno.FindAsync(id);
            if (izkladisceno == null)
            {
                return NotFound();
            }

            var currentUser = User.Identity?.Name;
            if (izkladisceno.Skladiscnik != currentUser)
            {
                return Unauthorized(); // Only the user who created the record can delete it
            }

            // Delete the entry
            _context.Izkladisceno.Remove(izkladisceno);
            await _context.SaveChangesAsync();

            // Redirect to the same page to show the updated list
            return RedirectToPage();
        }
    }

}