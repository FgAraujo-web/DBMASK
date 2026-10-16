using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace DBMASK.Web.Models;
public class AppUser : IdentityUser { }
public class Customer {
 public int Id {get;set;}
 [MaxLength(120)] public string Name {get;set;}="";
 [MaxLength(9)] public string Nif {get;set;}="";
 [MaxLength(200)] public string Email {get;set;}="";
 [MaxLength(20)] public string Phone {get;set;}="";
}
public enum MaskMethod { None=0, Full=1, Last4=2, EmailPartial=3, NamePartial=4 }
public class MaskRule {
 public int Id {get;set;}
 [MaxLength(80)] public string Resource {get;set;}="Customer";
 [MaxLength(80)] public string Field {get;set;}="";
 [MaxLength(80)] public string Role {get;set;}="";
 public MaskMethod Method {get;set;}
}
public class AuditEntry {
 public long Id {get;set;}
 public DateTime OccurredUtc {get;set;}=DateTime.UtcNow;
 [MaxLength(100)] public string ActorId {get;set;}="";
 [MaxLength(80)] public string Action {get;set;}="";
 [MaxLength(160)] public string Detail {get;set;}="";
}
public record CustomerView(int Id,string Name,string Nif,string Email,string Phone);
