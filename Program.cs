using DBMASK.Web.Data;
using DBMASK.Web.Models;
using DBMASK.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder=WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(o=>o.UseSqlServer(builder.Configuration.GetConnectionString("DBMASK")));
builder.Services.AddIdentity<AppUser,IdentityRole>(o=>{o.Password.RequiredLength=12;o.Lockout.MaxFailedAccessAttempts=5;o.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(15);o.User.RequireUniqueEmail=true;}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(o=>{o.LoginPath="/Account/Login";o.AccessDeniedPath="/Account/Denied";o.Cookie.HttpOnly=true;o.Cookie.SecurePolicy=CookieSecurePolicy.Always;o.Cookie.SameSite=SameSiteMode.Lax;});
builder.Services.AddScoped<MaskEngine>();builder.Services.AddScoped<CustomerReader>();
var app=builder.Build();
if(!app.Environment.IsDevelopment()){app.UseExceptionHandler("/Account/Denied");app.UseHsts();}
app.UseHttpsRedirection();app.UseStaticFiles();app.UseRouting();app.UseAuthentication();app.UseAuthorization();
app.MapControllerRoute("default","{controller=Customers}/{action=Index}/{id?}");
await Bootstrap.InitializeAsync(app.Services,app.Configuration);
app.Run();
