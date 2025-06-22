using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using diplomska.Data;
using diplomska.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Skladiščnik
{
    [Authorize(Roles = "Skladiščnik, Admin")]

    public class CreateModel : PageModel
    {
        private readonly diplomska.Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(diplomska.Data.ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var skladiščniki = await _userManager.GetUsersInRoleAsync("Skladiščnik");

            ViewData["DolocenSkladiscnikId"] = new SelectList(
                skladiščniki,
                "Id",
                "UserName"
            );

            return Page();
        }


        [BindProperty]
        public Transport Transport { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Transport.DolocenSkladiscnikId = currentUserId;

            _context.Transport.Add(Transport);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
