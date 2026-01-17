using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models;

public class PatientPortalAppointmentViewModel
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Range(0, 120)]
    public int Age { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Doctor")]
    public int DoctorId { get; set; }

    [Required]
    [Display(Name = "Date & Time")]
    public DateTime Date { get; set; }

    [Required]
    [Display(Name = "Reason for visit")]
    public string Reason { get; set; } = string.Empty;
}





