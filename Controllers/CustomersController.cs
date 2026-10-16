using DBMASK.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DBMASK.Web.Controllers;
[Authorize(Roles="Administrador,Gestor,Operador")]
public class CustomersController(CustomerReader reader):Controller {
 public async Task<IActionResult> Index()=>View(await reader.ReadAsync(User));
}
