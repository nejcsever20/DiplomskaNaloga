using System;
using System.Linq;
using System.Threading.Tasks;
using diplomska.Data;
using diplomska.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace diplomska.Pages.Izmenovodja
{
    [Authorize(Roles = "Admin, Izmenovodja")]

    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Transport Transport { get; set; }

        // OnGetAsync to load transport details
        public async Task<IActionResult> OnGetAsync(long id)  // Change type to long
        {
            Transport = await _context.Transport.FindAsync(id);  // Use long id for the lookup

            if (Transport == null)
            {
                return NotFound();
            }

            return Page();
        }

        // OnPostDeleteAsync to handle delete
        public async Task<IActionResult> OnPostDeleteAsync(long id)  // Change type to long
        {
            var transport = await _context.Transport.FindAsync(id);  // Use long id for the lookup

            if (transport == null)
            {
                return NotFound();
            }

            // Remove transport from DB
            _context.Transport.Remove(transport);
            await _context.SaveChangesAsync();

            // Redirect back to the transport list after deletion
            return RedirectToPage("./Index");
        }

    }
}
