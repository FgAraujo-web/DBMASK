using DBMASK.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace DBMASK.Web.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options):IdentityDbContext<AppUser>(options) {
 public DbSet<Customer> Customers => Set<Customer>();
 public DbSet<MaskRule> MaskRules => Set<MaskRule>();
 public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();
 protected override void OnModelCreating(ModelBuilder b) {
  base.OnModelCreating(b);
  b.Entity<MaskRule>().HasIndex(x=>new{x.Resource,x.Field,x.Role}).IsUnique();
 }
}
