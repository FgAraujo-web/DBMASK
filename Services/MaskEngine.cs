using DBMASK.Web.Models;
namespace DBMASK.Web.Services;
public class MaskEngine {
 public string Apply(string? value,MaskMethod method) {
  var v=value??"";
  if(method==MaskMethod.None || v.Length==0) return v;
  return method switch {
   MaskMethod.Full => new string('*',v.Length),
      MaskMethod.Last4 =>
       v.Length <= 4
           ? new string('*', v.Length)
           : new string('*', v.Length - 4) + v[^4..],
      MaskMethod.EmailPartial => MaskEmail(v),
   MaskMethod.NamePartial => v.Length<=1?"*":v[..1]+new string('*',v.Length-1),
   _ => new string('*',v.Length)
  };
 }
 private static string MaskEmail(string v) {
  var at=v.IndexOf('@');
  if(at<=0 || at==v.Length-1) return new string('*',v.Length);
  return v[..1]+new string('*',Math.Max(1,at-1))+v[at..];
 }
}
