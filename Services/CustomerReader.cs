
using DBMASK.Web.Data;
using DBMASK.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DBMASK.Web.Services;

public class CustomerReader(AppDbContext db, MaskEngine engine)
{
    public async Task<List<CustomerView>> ReadAsync(ClaimsPrincipal user)
    {
        // Identificar explicitamente o perfil autorizado
        string role;

        if (user.IsInRole("Administrador"))
            role = "Administrador";
        else if (user.IsInRole("Gestor"))
            role = "Gestor";
        else if (user.IsInRole("Operador"))
            role = "Operador";
        else
            throw new UnauthorizedAccessException(
                "O utilizador não possui um perfil autorizado.");

        // Consultar regras de mascaramento
        var rules = await db.MaskRules
            .AsNoTracking()
            .Where(x => x.Resource == "Customer" && x.Role == role)
            .ToDictionaryAsync(x => x.Field, x => x.Method);

        // Na ausência de uma regra, aplicar máscara completa
        MaskMethod Get(string field) =>
            rules.GetValueOrDefault(field, MaskMethod.Full);

        // Consultar clientes
        var customers = await db.Customers
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Take(100)
            .ToListAsync();

        // Aplicar mascaramento no servidor
        var result = customers.Select(x => new CustomerView(
            x.Id,
            engine.Apply(x.Name, Get("Name")),
            engine.Apply(x.Nif, Get("Nif")),
            engine.Apply(x.Email, Get("Email")),
            engine.Apply(x.Phone, Get("Phone"))
        )).ToList();

        // Registar auditoria sem incluir dados sensíveis
        db.AuditEntries.Add(new AuditEntry
        {
            ActorId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? "unknown",
            Action = "Customers.Read",
            Detail = $"Role={role}; Count={result.Count}"
        });

        await db.SaveChangesAsync();

        return result;
    }
}
