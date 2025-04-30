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
    }
}
