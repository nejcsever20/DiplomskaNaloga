using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;

namespace diplomska.Pages.Skladiščnik
{
    public class DeleteModel : PageModel
    {
        private readonly diplomska.Data.ApplicationDbContext _context;

        public DeleteModel(diplomska.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Transport Transport { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transport = await _context.Transport.FirstOrDefaultAsync(m => m.Id == id);

            if (transport == null)
            {
                return NotFound();
            }
            else
            {
                Transport = transport;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transport = await _context.Transport.FindAsync(id);
            if (transport != null)
            {
                Transport = transport;
                _context.Transport.Remove(Transport);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
