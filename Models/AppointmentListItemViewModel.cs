namespace HospitalManagementSystem.Models;

public class AppointmentListItemViewModel
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; }
}





