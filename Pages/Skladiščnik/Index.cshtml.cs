using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Skladiščnik
{
    [Authorize(Roles = "Skladiščnik, Admin")]

    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Transport> Transport { get; set; } = new List<Transport>();

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Transport.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(t =>
                    t.StTransporta.ToString().Contains(SearchString) ||
                    t.Registracija.Contains(SearchString) ||
                    t.Voznik.Contains(SearchString));
            }

            Transport = await query.ToListAsync();
        }
    }
}
