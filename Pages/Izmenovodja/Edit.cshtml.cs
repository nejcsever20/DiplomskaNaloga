using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;


namespace diplomska.Pages.Izmenovodja
{
    public class EditModel : PageModel
    {
        private readonly diplomska.Data.ApplicationDbContext _context;

        public EditModel(diplomska.Data.ApplicationDbContext context)
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
            Transport = transport;
            ViewData["DolocenSkladiscnikId"] = new SelectList(_context.Users, "Id", "UserName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Transport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransportExists(Transport.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TransportExists(long? id)
        {
            return _context.Transport.Any(e => e.Id == id);
        }
    }
}
