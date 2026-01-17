using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[AllowAnonymous]
public class PatientPortalController : Controller
{
    private readonly HospitalDataService _dataService;

    public PatientPortalController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet]
    public IActionResult BookAppointment()
    {
        ViewBag.Doctors = _dataService.GetDoctors().ToList();
        return View(new PatientPortalAppointmentViewModel
        {
            Date = DateTime.Now.AddHours(1)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BookAppointment(PatientPortalAppointmentViewModel model)
    {
        ViewBag.Doctors = _dataService.GetDoctors().ToList();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // create or store patient
        var patient = new Patient
        {
            FullName = model.FullName,
            Age = model.Age,
            Gender = model.Gender,
            Phone = model.Phone,
            Address = model.Address
        };
        patient = _dataService.AddPatient(patient);

        try
        {
            var appointment = new Appointment
            {
                PatientId = patient.Id,
                DoctorId = model.DoctorId,
                Date = model.Date,
                Reason = model.Reason,
                Status = AppointmentStatus.Pending
            };

            var createdAppointment = _dataService.AddAppointment(appointment);

            // Notify administrators and managers about the new appointment
            var adminsAndManagers = _dataService.GetUsers()
                .Where(u => (u.Role == UserRole.Administrator || u.Role == UserRole.Manager) && u.IsActive)
                .ToList();

            foreach (var user in adminsAndManagers)
            {
                _dataService.AddNotification(new Notification
                {
                    UserId = user.Id,
                    Title = "New Appointment Request",
                    Message = $"Patient {patient.FullName} has requested an appointment with Dr. {_dataService.GetDoctor(model.DoctorId)?.FullName ?? "Unknown"} on {model.Date:MMM dd, yyyy HH:mm}",
                    Type = NotificationType.ApprovalRequired,
                    ActionUrl = $"/Appointments/Edit/{createdAppointment.Id}",
                    RelatedEntityId = createdAppointment.Id,
                    RelatedEntityType = "Appointment"
                });
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        TempData["SuccessMessage"] = "Appointment submitted successfully and is pending approval. You will be notified once it's approved.";
        return RedirectToAction("Confirmation");
    }

    [HttpGet]
    public IActionResult Confirmation()
    {
        return View();
    }
}





