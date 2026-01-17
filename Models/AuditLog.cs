namespace HospitalManagementSystem.Models;

public enum AuditAction
{
    Create,
    Update,
    Delete,
    View,
    Approve,
    Reject,
    Login,
    Logout
}

public class AuditLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

