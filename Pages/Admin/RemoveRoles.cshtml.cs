using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace diplomska.Pages.Admin
{
    public class RemoveRolesModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RemoveRolesModel(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [BindProperty]
        public string NewRoleName { get; set; }

        [BindProperty]
        public string RoleToDelete { get; set; }

        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

        public bool CreateRoleSuccess { get; set; }
        public string CreateRoleError { get; set; }

        public bool DeleteRoleSuccess { get; set; }
        public string DeleteRoleError { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadRoles();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateRoleAsync()
        {
            if (string.IsNullOrEmpty(NewRoleName))
            {
                CreateRoleError = "Ime vloge ne sme biti prazno.";
                return Page();
            }

            var roleExist = await _roleManager.RoleExistsAsync(NewRoleName);
            if (roleExist)
            {
                CreateRoleError = "Vloga že obstaja.";
                return Page();
            }

            var newRole = new IdentityRole(NewRoleName);
            var result = await _roleManager.CreateAsync(newRole);

            if (result.Succeeded)
            {
                CreateRoleSuccess = true;
                CreateRoleError = null;
                NewRoleName = string.Empty; // Reset input after success
            }
            else
            {
                CreateRoleError = "Prišlo je do napake pri ustvarjanju vloge.";
            }

            await LoadRoles();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteRoleAsync()
        {
            if (string.IsNullOrEmpty(RoleToDelete))
            {
                DeleteRoleError = "Izberite vlogo za izbris.";
                return Page();
            }

            var role = await _roleManager.FindByNameAsync(RoleToDelete);

            if (role == null)
            {
                DeleteRoleError = "Vloga ne obstaja.";
                return Page();
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                DeleteRoleSuccess = true;
                DeleteRoleError = null;
            }
            else
            {
                DeleteRoleError = "Prišlo je do napake pri brisanju vloge.";
            }

            await LoadRoles();
            return Page();
        }

        private async Task LoadRoles()
        {
            Roles = _roleManager.Roles
                .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
                .ToList();
        }
    }
}

