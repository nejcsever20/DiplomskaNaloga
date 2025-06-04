using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using diplomska.Data;
using diplomska.Models;

namespace diplomska.Pages.Izmenovodja
{
    public class CallbackModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CallbackModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class TransportViewModel
        {
            public Transport? Transport { get; set; }
            public string? CallbackReason { get; set; }
        }

        public List<TransportViewModel> Transports { get; set; } = new();

        // Predefined callback reasons
        public List<string> CallbackReasons { get; } = new()
        {
            "Duplicate StTransporta",
            "Delayed Transport",
            "Damaged Goods",
            "Incorrect Documentation",
            "Other"
        };

        [BindProperty]
        public long SelectedTransportId { get; set; }

        [BindProperty]
        public string SelectedReason { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            Transports = await _context.Transport
                .OrderByDescending(t => t.PlaniranPrihod)
                .Select(t => new TransportViewModel
                {
                    Transport = t,
                    CallbackReason = t.IsCallback ? t.SK : null
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostSubmitCallbackAsync()
        {
            if (SelectedTransportId <= 0 || string.IsNullOrWhiteSpace(SelectedReason))
            {
                ModelState.AddModelError(string.Empty, "Please select a valid transport and reason.");
                await OnGetAsync(); // repopulate Transports for redisplay
                return Page();
            }

            var transport = await _context.Transport.FindAsync(SelectedTransportId);

            if (transport == null)
            {
                ModelState.AddModelError(string.Empty, "Selected transport not found.");
                await OnGetAsync(); // repopulate
                return Page();
            }

            transport.IsCallback = true;
            transport.SK = SelectedReason;

            _context.Transport.Update(transport);
            await _context.SaveChangesAsync();

            return RedirectToPage(); // redirect to clear form state and reload data
        }

        public async Task<IActionResult> OnPostRemoveCallbackAsync(long transportId)
        {
            var transport = await _context.Transport.FindAsync(transportId);

            if (transport == null)
            {
                ModelState.AddModelError(string.Empty, "Transport not found");

                await OnGetAsync();
                return Page();
            }

            transport.IsCallback = false;
            transport.SK = null;

            _context.Transport.Update(transport);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
