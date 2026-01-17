using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models;

public enum TaskStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public enum TaskPriority
{
    Low,
    Medium,
    High,
    Urgent
}

public class Task
{
    public int Id { get; set; }
    
    [Required]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;
    
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Display(Name = "Assign To")]
    public int AssignedToUserId { get; set; }
    
    public int? AssignedByUserId { get; set; }
    
    [Display(Name = "Status")]
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    
    [Display(Name = "Priority")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    
    [Display(Name = "Due Date")]
    public DateTime? DueDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }
    public string? RelatedEntityType { get; set; } // e.g., "Appointment", "Patient"
    public int? RelatedEntityId { get; set; }
}

