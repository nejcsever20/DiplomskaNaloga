using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Skladiščnik
{
    [Authorize(Roles = "Skladiščnik, Admin")]

    public class DetailsModel : PageModel
    {
        private readonly diplomska.Data.ApplicationDbContext _context;

        public DetailsModel(diplomska.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
