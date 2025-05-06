using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Izmenovodja
{
    [Authorize] // ensures only logged-in users can access this page
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Transport> Transport { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Transport = await _context.Transport
                .Include(t => t.DolocenSkladiscnikNavigation)
                .ToListAsync();
        }

        public async Task<IActionResult> OnGetGetTransportInfo(int id)
        {
            var transport = await _context.Transport
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    stTransporta = t.StTransporta,
                    registracija = t.Registracija,
                    voznik = t.Voznik
                })
                .FirstOrDefaultAsync();

            if (transport == null)
                return new JsonResult(new { error = "Transport not found" });

            return new JsonResult(transport);
        }

        //This method will handle update sp line directly into the database
        public async Task<IActionResult> OnPostUpdateSpAsync(int id, bool sp)
        {
            var transport = await _context.Transport.FindAsync(id);
            if(transport != null)
            {
                transport.Sp = sp; // update the vlaue!
                await _context.SaveChangesAsync();
                return new JsonResult(new
                {
                    success = true
                });
            }

            return new JsonResult(new
            {
                success = false
            });
        }
    }
}
