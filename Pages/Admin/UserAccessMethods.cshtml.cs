using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using diplomska.Models;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authorization;

namespace diplomska.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserAccessMethodsModel : PageModel
    {

        private readonly UserManager<IdentityUser> _userManager;

        public UserAccessMethodsModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<UserLoginInfoDisplay> UsersWithLoginMethods { get; set; }

        public class UserLoginInfoDisplay
        {
            public string? UserName { get; set; }
            public string? Email { get; set; }
            public string? LoginProvider { get; set; }
        }

        public async Task OnGetAsync()
        {
            UsersWithLoginMethods = new List<UserLoginInfoDisplay>();

            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                var logins = await _userManager.GetLoginsAsync(user);

                if (logins.Any())
                {
                    foreach (var login in logins)
                    {
                        UsersWithLoginMethods.Add(new UserLoginInfoDisplay
                        {
                            UserName = user.UserName,
                            Email = user.Email,
                            LoginProvider = login.LoginProvider,
                        });
                    }
                }
                else
                {
                    UsersWithLoginMethods.Add(new UserLoginInfoDisplay
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        LoginProvider = "Local"
                    });
                }
            }
        }
    }
}