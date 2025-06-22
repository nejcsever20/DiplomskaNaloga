using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Analitika
{
    [Authorize(Roles = "Admin, Analitika")]

    public class CompareCallbackModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CompareCallbackModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Transport? NewTransport { get; set; }
        public ArchivedTransport? OldTransport { get; set; }

        [BindProperty(SupportsGet = true)]
        public long? ArchivedId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (ArchivedId == null)
            {
                return NotFound();
            }

            // Load the archived transport
            OldTransport = await _context.ArchivedTransports
    .AsNoTracking()
    .FirstOrDefaultAsync(a => a.Id == ArchivedId);

            if (OldTransport == null)
            {
                return NotFound();
            }

            // Load the latest new transport with matching StTransporta
            if (OldTransport.StTransporta != null)
            {
                NewTransport = await _context.Transport
                    .AsNoTracking()
                    .Include(t => t.DolocenSkladiscnikNavigation)
                    .Where(t => t.StTransporta == OldTransport.StTransporta && !t.IsArchived)
                    .OrderByDescending(t => t.PlaniranPrihod)
                    .FirstOrDefaultAsync();
            }

            return Page();
        }
    }
}
