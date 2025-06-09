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
        public string? CallbackReason { get; set; }

        public async Task OnGetAsync()
        {
            Transports = await _context.Transport
                .Where(t => !t.IsArchived) // 👈 Only show active ones
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
                await OnGetAsync();
                return Page();
            }

            var transport = await _context.Transport.FindAsync(SelectedTransportId);
            if (transport == null)
            {
                ModelState.AddModelError(string.Empty, "Selected transport not found.");
                await OnGetAsync();
                return Page();
            }

            var archived = new ArchivedTransport
            {
                StTransporta = transport.StTransporta,
                PlaniranPrihod = transport.PlaniranPrihod,
                Sp = transport.Sp,
                Skladisce = transport.Skladisce,
                VrstaTransporta = transport.VrstaTransporta,
                PavzaVoznika = transport.PavzaVoznika,
                DolocenSkladiscnikId = transport.DolocenSkladiscnikId,
                Registracija = transport.Registracija,
                VrstaPrevoznegaSredstva = transport.VrstaPrevoznegaSredstva,
                Voznik = transport.Voznik,
                NAVISZacetekSklada = transport.NAVISZacetekSklada,
                NAVISKonecSklada = transport.NAVISKonecSklada,
                CallbackReason = SelectedReason,
                IsCallback = true
            };

            await _context.ArchivedTransports.AddAsync(archived);

            // Update the original transport
            transport.IsCallback = true;
            transport.SK = SelectedReason;
            transport.IsArchived = true;

            _context.Transport.Update(transport);
            await _context.SaveChangesAsync();

            return RedirectToPage();

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
