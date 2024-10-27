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

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

        // Email sending logic
        private async Task SendEmailAsync(string email, string callbackUrl)
        {
            using (var client = new SmtpClient("smtp.your-email-provider.com") // Replace with your SMTP server details
            {
                Port = 587, // Adjust as necessary
                Credentials = new NetworkCredential("your-email@example.com", "your-email-password"), // Replace with your email and password
                EnableSsl = true,
            })
            {
                var message = new MailMessage
                {
                    From = new MailAddress("your-email@example.com"), // Replace with your email
                    Subject = "Confirm your email",
                    Body = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.",
                    IsBodyHtml = true,
                };
                message.To.Add(email);
                await client.SendMailAsync(message);
            }
        }

        // GET: /Account/ConfirmEmail
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // Other actions remain unchanged...

        // GET: /Account/ForgotPassword
        public IActionResult ForgotPassword() { return View(); }

        // Remaining methods from your previous implementation...
    }
}
