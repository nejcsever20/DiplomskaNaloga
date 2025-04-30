using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using diplomska.Data;
using diplomska.Models;

namespace diplomska.Pages.Skladiščnik
{
    public class CreateModel : PageModel
    {
        private readonly diplomska.Data.ApplicationDbContext _context;

        public CreateModel(diplomska.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["DolocenSkladiscnikId"] = new SelectList(_context.Users, "Id", "UserName");
            return Page();
        }

        [BindProperty]
        public Transport Transport { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Transport.Add(Transport);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
