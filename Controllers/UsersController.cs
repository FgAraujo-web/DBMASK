
using DBMASK.Web.Data;
using DBMASK.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DBMASK.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class UsersController(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager,
    AppDbContext db) : Controller
{
    private static readonly string[] AllowedRoles =
    {
        "Administrador", "Gestor", "Operador"
    };

    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.Email)
            .ToListAsync();

        var result = new List<UserListViewModel>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);

            var isLocked = await userManager.IsLockedOutAsync(user);

            result.Add(new UserListViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Role = string.Join(", ", roles),
                IsLocked = isLocked,
                LockoutEnd = isLocked ? user.LockoutEnd : null
            });
        }

        return View(result);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateUserViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!AllowedRoles.Contains(model.Role))
            ModelState.AddModelError(
                nameof(model.Role), "Perfil inválido.");

        if (!ModelState.IsValid)
            return View(model);

        var email = model.Email.Trim();

        if (await userManager.FindByEmailAsync(email) != null)
        {
            ModelState.AddModelError(
                nameof(model.Email), "Este email já está registado.");

            return View(model);
        }

        if (!await roleManager.RoleExistsAsync(model.Role))
        {
            ModelState.AddModelError(
                nameof(model.Role), "O perfil não existe.");

            return View(model);
        }

        var user = new AppUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var created = await userManager.CreateAsync(
            user, model.Password);

        if (!created.Succeeded)
        {
            foreach (var error in created.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        var assigned = await userManager.AddToRoleAsync(
            user, model.Role);

        if (!assigned.Succeeded)
        {
            await userManager.DeleteAsync(user);

            ModelState.AddModelError(
                "", "Não foi possível atribuir o perfil.");

            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest();

        var user = await userManager.FindByIdAsync(id);

        if (user == null)
            return NotFound();

        if (!await userManager.IsLockedOutAsync(user))
        {
            TempData["Info"] = "A conta não está bloqueada.";
            return RedirectToAction(nameof(Index));
        }

        var unlockResult =
            await userManager.SetLockoutEndDateAsync(user, null);

        if (!unlockResult.Succeeded)
        {
            TempData["Error"] =
                "Não foi possível desbloquear a conta.";

            return RedirectToAction(nameof(Index));
        }

        var resetResult =
            await userManager.ResetAccessFailedCountAsync(user);

        if (!resetResult.Succeeded)
        {
            TempData["Error"] =
                "O bloqueio foi removido, mas não foi possível repor o contador de tentativas.";

            return RedirectToAction(nameof(Index));
        }

        db.AuditEntries.Add(new AuditEntry
        {
            ActorId = User.FindFirstValue(
                ClaimTypes.NameIdentifier) ?? "unknown",

            Action = "User.Unlock",

            Detail = $"TargetUserId={user.Id}; Manual unlock"
        });

        await db.SaveChangesAsync();

        TempData["Success"] =
            "Conta desbloqueada com sucesso.";

        return RedirectToAction(nameof(Index));
    }
}

public class UserListViewModel
{
    public string Id { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public bool IsLocked { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
}
