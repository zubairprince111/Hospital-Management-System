using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models;

public class Report
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty; // e.g., "Appointments", "Patients", "Tasks"
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
    public int GeneratedByUserId { get; set; }
    public string Data { get; set; } = string.Empty; // JSON or formatted data
}

public class ReportViewModel
{
    [Required]
    [Display(Name = "Report Type")]
    public string ReportType { get; set; } = string.Empty;
    
    [Display(Name = "Start Date")]
    public DateTime? StartDate { get; set; }
    
    [Display(Name = "End Date")]
    public DateTime? EndDate { get; set; }
    
    public Dictionary<string, object> Summary { get; set; } = new();
    public List<Dictionary<string, object>> Details { get; set; } = new();
}

