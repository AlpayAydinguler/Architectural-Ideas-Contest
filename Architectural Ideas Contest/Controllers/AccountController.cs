using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Architectural_Ideas_Contest.Models; // Adjust this if necessary
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Architectural_Ideas_Contest.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager; // Add RoleManager

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager; // Initialize RoleManager
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign default role (Candidate) after successful registration
                    await _userManager.AddToRoleAsync(user, Roles.Candidate);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail", // Adjust the path as needed
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code)) },
                        protocol: Request.Scheme);

                    // Send the email
                    await SendEmailAsync(model.Email, callbackUrl);

                    return RedirectToAction("RegisterConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // Other actions remain unchanged...

        // Add a method to create roles if they don't exist
        public async Task<IActionResult> CreateRoles()
        {
            if (!await _roleManager.RoleExistsAsync(Roles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }
            if (!await _roleManager.RoleExistsAsync(Roles.Judge))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.Judge));
            }
            if (!await _roleManager.RoleExistsAsync(Roles.Candidate))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.Candidate));
            }

            return Ok("Roles created if they didn't exist.");
        }

        // Remaining methods...
    }
}
