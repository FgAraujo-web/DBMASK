
using DBMASK.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DBMASK.Web.Data;

public static class Bootstrap
{
    public static async Task InitializeAsync(
        IServiceProvider services,
        IConfiguration cfg)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        // 1. Aplicar as migrations pendentes
        await db.Database.MigrateAsync();

        // 2. Criar os perfis de acesso
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        string[] allowedRoles =
        {
            "Administrador",
            "Gestor",
            "Operador"
        };

        foreach (var role in allowedRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Não foi possível criar o perfil '{role}': " +
                        string.Join("; ",
                            result.Errors.Select(e => e.Description)));
                }
            }
        }

        // 3. Criar as regras iniciais de mascaramento
        // Apenas quando ainda não existem regras
        if (!await db.MaskRules.AnyAsync())
        {
            string[] fields =
            {
                "Name",
                "Nif",
                "Email",
                "Phone"
            };

            foreach (var role in allowedRoles)
            {
                foreach (var field in fields)
                {
                    MaskMethod method;

                    if (role == "Administrador")
                    {
                        method = MaskMethod.None;
                    }
                    else if (role == "Gestor")
                    {
                        method = field switch
                        {
                            "Name" => MaskMethod.None,
                            "Email" => MaskMethod.EmailPartial,
                            _ => MaskMethod.Last4
                        };
                    }
                    else
                    {
                        method = MaskMethod.Full;
                    }

                    db.MaskRules.Add(new MaskRule
                    {
                        Resource = "Customer",
                        Role = role,
                        Field = field,
                        Method = method
                    });
                }
            }

            await db.SaveChangesAsync();
        }

        // 4. Obter as credenciais iniciais do Administrador
        // Em desenvolvimento, configurar através de User Secrets
        var adminEmail = cfg["Bootstrap:AdminEmail"];
        var adminPassword = cfg["Bootstrap:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) ||
            string.IsNullOrWhiteSpace(adminPassword))
        {
            // Sem credenciais configuradas, não criar
            // automaticamente uma conta Administrador.
            return;
        }

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<AppUser>>();

        // 5. Verificar se o Administrador já existe
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var created = await userManager.CreateAsync(
                admin,
                adminPassword);

            if (!created.Succeeded)
            {
                throw new InvalidOperationException(
                    "Não foi possível criar o Administrador: " +
                    string.Join("; ",
                        created.Errors.Select(e => e.Description)));
            }
        }

        // 6. Garantir a atribuição do perfil Administrador
        if (!await userManager.IsInRoleAsync(
                admin, "Administrador"))
        {
            var assigned = await userManager.AddToRoleAsync(
                admin,
                "Administrador");

            if (!assigned.Succeeded)
            {
                throw new InvalidOperationException(
                    "Não foi possível atribuir o perfil Administrador: " +
                    string.Join("; ",
                        assigned.Errors.Select(e => e.Description)));
            }
        }
    }
}
