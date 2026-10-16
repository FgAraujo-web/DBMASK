
using DBMASK.Web.Data;
using DBMASK.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBMASK.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class AuditController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(
        string? actor,
        [FromQuery(Name = "action")] string? operation,
        DateTime? startDate,
        DateTime? endDate,
        int page = 1)
    {
        const int pageSize = 20;

        var connection = db.Database.GetDbConnection();

        var databaseName = connection.Database;
        var serverName = connection.DataSource;

        var auditCount = await db.AuditEntries.CountAsync();

        Console.WriteLine("===== DBMASK DIAGNÓSTICO =====");
        Console.WriteLine($"Servidor: {serverName}");
        Console.WriteLine($"Base de dados: {databaseName}");
        Console.WriteLine($"Registos de auditoria: {auditCount}");
        Console.WriteLine("==============================");
        Console.WriteLine("===== FILTROS AUDITORIA =====");
        Console.WriteLine($"Actor: [{actor ?? "NULL"}]");
        Console.WriteLine($"Action: [{operation ?? "NULL"}]");
        Console.WriteLine($"StartDate: [{startDate?.ToString() ?? "NULL"}]");
        Console.WriteLine($"EndDate: [{endDate?.ToString() ?? "NULL"}]");
        Console.WriteLine("=============================");


        page = Math.Max(1, page);

        var query = db.AuditEntries
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(actor))
        {
            var userIds = db.Users
                .Where(u => u.Email != null &&
                    u.Email.Contains(actor))
                .Select(u => u.Id);

            query = query.Where(a =>
                a.ActorId.Contains(actor) ||
                userIds.Contains(a.ActorId));
        }

        if (!string.IsNullOrWhiteSpace(operation))
        {
            query = query.Where(a => a.Action == operation);
        }

        if (startDate.HasValue)
        {
            var start = DateTime.SpecifyKind(
                startDate.Value.Date, DateTimeKind.Utc);

            query = query.Where(a => a.OccurredUtc >= start);
        }

        if (endDate.HasValue)
        {
            var end = DateTime.SpecifyKind(
                endDate.Value.Date.AddDays(1), DateTimeKind.Utc);

            query = query.Where(a => a.OccurredUtc < end);
        }

        var totalRecords = await query.CountAsync();

        var totalPages = Math.Max(1,
            (int)Math.Ceiling(totalRecords / (double)pageSize));

        page = Math.Min(page, totalPages);

        var entries = await query
            .OrderByDescending(a => a.OccurredUtc)
            .ThenByDescending(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var actorIds = entries
            .Select(a => a.ActorId)
            .Distinct()
            .ToList();

        var users = await db.Users
            .AsNoTracking()
            .Where(u => actorIds.Contains(u.Id))
            .Select(u => new { u.Id, u.Email })
            .ToDictionaryAsync(u => u.Id,
                u => u.Email ?? "");

        var records = entries.Select(a =>
            new AuditRecordViewModel
            {
                Id = a.Id,
                OccurredUtc = a.OccurredUtc,
                ActorId = a.ActorId,
                ActorEmail = users.GetValueOrDefault(
                    a.ActorId, "Utilizador desconhecido"),
                Action = a.Action,
                Detail = a.Detail
            }).ToList();

        return View(new AuditFilterViewModel
        {
            Actor = actor,
            Action = operation,
            StartDate = startDate,
            EndDate = endDate,
            Page = page,
            TotalPages = totalPages,
            TotalRecords = totalRecords,
            Records = records
        });
    }
}
