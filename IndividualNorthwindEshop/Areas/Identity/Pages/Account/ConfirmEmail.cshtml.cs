using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using CommonData.Models;
using Microsoft.Extensions.Logging;
namespace IndividualNorthwindEshop.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(UserManager<User> userManager, ILogger<ConfirmEmailModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                _logger.LogWarning("ConfirmEmail: userId or code is null");
                return RedirectToPage("/Error", new { message = "Invalid email confirmation link." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("ConfirmEmail: Unable to load user with ID '{userID}'", userId);
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            _logger.LogInformation("ConfirmEmail: StatusMessage = {StatusMessage}", StatusMessage);

            return Page();
        }
    }
}
