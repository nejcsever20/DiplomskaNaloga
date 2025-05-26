using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using diplomska.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using diplomska.Data;

namespace diplomska.Pages.Izmenovodja
{
    [Authorize] // Allow all authenticated users to access the page
    public class DetailedInfoModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DetailedInfoModel(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<Izkladisceno> IzkladiscenoItems { get; set; } = new();
        public List<IdentityUser> Skladiscniki { get; set; } = new();
        public Transport? Transport { get; set; }

        public async Task<IActionResult> OnGetAsync(int? transportId)
        {
            if (transportId == null)
                return NotFound();

            Transport = await _context.Transport
                .FirstOrDefaultAsync(t => t.Id == transportId);

            if (Transport == null)
                return NotFound();

            IzkladiscenoItems = await _context.Izkladisceno
                .Where(x => x.TransportId == transportId)
                .OrderByDescending(x => x.Datum)
                .ToListAsync();

            var users = await _userManager.GetUsersInRoleAsync("Skladiščnik");
            Skladiscniki = users.ToList();

            return Page();
        }
    }
}
