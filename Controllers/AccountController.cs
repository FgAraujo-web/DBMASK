
using DBMASK.Web.Data;
using DBMASK.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DBMASK.Web.Controllers;

public class AccountController(
    SignInManager<AppUser> signIn,
    UserManager<AppUser> userManager,
    AppDbContext db) : Controller
{
    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : null;

        return View();
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string email,
        string password,
        string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                "", "Credenciais inválidas.");

            return View();
        }

        var user = await userManager.FindByEmailAsync(email);

        var result = await signIn.PasswordSignInAsync(
            email,
            password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            db.AuditEntries.Add(new AuditEntry
            {
                ActorId = user?.Id ?? "unknown",
                Action = result.IsLockedOut
                    ? "Auth.Lockout"
                    : "Auth.LoginFailed",

                Detail = user == null
                    ? "Unknown account"
                    : result.IsLockedOut
                        ? "Account locked after failed login attempts"
                        : result.RequiresTwoFactor
                            ? "Additional verification required"
                            : "Authentication failed"
            });

            await db.SaveChangesAsync();

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    "",
                    "A sua conta foi temporariamente bloqueada " +
                    "após várias tentativas de autenticação falhadas. " +
                    "Tente novamente dentro de 15 minutos."
                );
            }
            else
            {
                ModelState.AddModelError(
                    "",
                    "Credenciais inválidas."
                );
            }

            return View();
        }

        db.AuditEntries.Add(new AuditEntry
        {
            ActorId = user?.Id ?? "unknown",
            Action = "Auth.Login",
            Detail = "Successful authentication"
        });

        await db.SaveChangesAsync();

        return LocalRedirect(
            Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : "/Customers");
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        db.AuditEntries.Add(new AuditEntry
        {
            ActorId = User.FindFirstValue(
                ClaimTypes.NameIdentifier) ?? "unknown",
            Action = "Auth.Logout",
            Detail = "User signed out"
        });

        await db.SaveChangesAsync();

        await signIn.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Denied()
    {
        return Content("Acesso não autorizado.");
    }
}
