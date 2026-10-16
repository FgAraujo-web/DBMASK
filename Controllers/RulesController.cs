
using DBMASK.Web.Data;
using DBMASK.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DBMASK.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class RulesController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var rules = await db.MaskRules
            .AsNoTracking()
            .OrderBy(x => x.Role)
            .ThenBy(x => x.Field)
            .ToListAsync();

        return View(rules);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Change(int id, MaskMethod method)
    {
        if (!Enum.IsDefined(method))
            return BadRequest("Método de mascaramento inválido.");

        var rule = await db.MaskRules.FindAsync(id);

        if (rule == null)
            return NotFound();

        if (rule.Method == method)
        {
            TempData["Info"] =
                "A regra já utiliza o método selecionado.";

            return RedirectToAction(nameof(Index));
        }

        var previous = rule.Method;

        rule.Method = method;

        db.AuditEntries.Add(new AuditEntry
        {
            ActorId = User.FindFirstValue(
                ClaimTypes.NameIdentifier) ?? "unknown",

            Action = "Rule.Change",

            Detail =
                $"RuleId={rule.Id}; " +
                $"Role={rule.Role}; " +
                $"Field={rule.Field}; " +
                $"{previous}->{method}"
        });

        await db.SaveChangesAsync();

        TempData["Success"] =
            "Regra de mascaramento atualizada com sucesso.";

        return RedirectToAction(nameof(Index));
    }
}
