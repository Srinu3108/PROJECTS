using FarmManagement.Web.Models.Interfaces;
using FarmManagement.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FarmManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = await _accountService.AuthenticateAsync(vm.Email, vm.Password);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(vm);
        }

        if (user.IsBlocked)
        {
            ModelState.AddModelError(string.Empty, "Your account has been suspended. Contact your administrator.");
            return View(vm);
        }

        if (user.Role == Models.Enums.UserRole.Admin)
        {
            var adminCount = await _accountService.GetAdminCountAsync();
            if (adminCount > 5)
            {
                ModelState.AddModelError(string.Empty, "Maximum admin accounts (5) reached. Contact system administrator.");
                return View(vm);
            }
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = vm.RememberMe });

        TempData["Success"] = $"Welcome back, {user.FullName}!";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (await _accountService.EmailExistsAsync(vm.Email))
        {
            ModelState.AddModelError("Email", "This email address is already registered.");
            return View(vm);
        }

        if (vm.Role == Models.Enums.UserRole.Admin)
        {
            var adminCount = await _accountService.GetAdminCountAsync();
            if (adminCount >= 5)
            {
                ModelState.AddModelError("Role", "Maximum admin accounts (5) reached. Choose a different role.");
                return View(vm);
            }
        }

        var success = await _accountService.RegisterAsync(vm);
        if (!success)
        {
            TempData["Error"] = "Registration failed. Please try again.";
            return View(vm);
        }

        TempData["Success"] = "Account created successfully. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var hint = await _accountService.GetPasswordHintAsync(vm.Email);
        if (hint == null)
        {
            ModelState.AddModelError(string.Empty, "No account found with that email.");
            return View(vm);
        }

        return RedirectToAction(nameof(ResetPassword), new { email = vm.Email });
    }

    public IActionResult ResetPassword(string email)
    {
        if (string.IsNullOrEmpty(email))
            return RedirectToAction(nameof(ForgotPassword));

        return View(new ResetPasswordViewModel { Email = email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var success = await _accountService.ResetPasswordAsync(vm.Email, vm.Hint, vm.NewPassword);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "The hint you entered is incorrect.");
            return View(vm);
        }

        TempData["Success"] = "Password reset successfully. Please log in with your new password.";
        return RedirectToAction(nameof(Login));
    }
}
