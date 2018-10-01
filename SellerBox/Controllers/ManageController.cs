using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SellerBox.Models.Database;
using SellerBox.ViewModels;

namespace SellerBox.Controllers
{
    public class ManageController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;

        private bool IsEmailValid(string email)
        {
            string pattern = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
            Match match = Regex.Match(email.Trim(), pattern, RegexOptions.IgnoreCase);

            return match.Success;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public ManageController(UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            Users user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            ManageUserViewModel model = new ManageUserViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                StatusMessage = StatusMessage,
                CurrentLogins = await _userManager.GetLoginsAsync(user)
            };
            
            model.OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            model.ShowRemoveButton = model.CurrentLogins.Count > 0;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ManageUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Users user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            
            if (model.Email != user.Email)
            {
                if (!IsEmailValid(model.Email))
                {
                    StatusMessage = "Ошибка: некорректный адрес электронной почты.";
                    return RedirectToAction(nameof(Index));
                }
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
            }

            if (model.PhoneNumber != user.PhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
            }

            StatusMessage = "Профиль обновлён";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            Users user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            var claim = info.Principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            user.IdVk = int.Parse(claim.Value);
            IdentityResult result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "Аккаунт успешно привязан";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            Users user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            user.IdVk = 0;
            IdentityResult result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            
            await _signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Аккаунт успешно отвязан";
            return RedirectToAction(nameof(Index));
        }
    }
}