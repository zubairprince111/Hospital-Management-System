namespace HospitalManagementSystem.Models;

public enum AppointmentStatus
{
    Pending,
    Approved,
    Cancelled
}

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
}


