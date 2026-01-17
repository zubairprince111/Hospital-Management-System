namespace HospitalManagementSystem.Models;

public class DashboardViewModel
{
    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public int TotalAppointments { get; set; }
    public List<Appointment> UpcomingAppointments { get; set; } = new();
}





