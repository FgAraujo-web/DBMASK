
using System.ComponentModel.DataAnnotations;

namespace DBMASK.Web.Models;

public class AuditFilterViewModel
{
    public string? Actor { get; set; }
    public string? Action { get; set; }

    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }

    public List<AuditRecordViewModel> Records { get; set; } = new();
}

public class AuditRecordViewModel
{
    public long Id { get; set; }
    public DateTime OccurredUtc { get; set; }
    public string ActorId { get; set; } = "";
    public string ActorEmail { get; set; } = "";
    public string Action { get; set; } = "";
    public string Detail { get; set; } = "";
}
