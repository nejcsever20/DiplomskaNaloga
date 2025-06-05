using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace diplomska.Pages.Izmenovodja
{
    public class ArchivedTransportsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ArchivedTransportsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ArchivedTransport> ArchivedTransports { get; set; } = new();

        public async Task OnGetAsync()
        {
            ArchivedTransports = await _context.ArchivedTransports
                .OrderByDescending(t => t.PlaniranPrihod)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostRemoveArchiveAsync(long id)
        {
            var archived = await _context.ArchivedTransports.FindAsync(id);

            if(archived == null)
            {
                ModelState.AddModelError(string.Empty, "Archived transport not found");
                return Page();
            }

            _context.ArchivedTransports.Remove(archived);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
