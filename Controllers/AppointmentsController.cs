using System.Security.Claims;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Authorize(Policy = "StaffOrAbove")]
public class AppointmentsController : Controller
{
    private readonly HospitalDataService _dataService;

    public AppointmentsController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index(string searchTerm = "")
    {
        IEnumerable<Appointment> appointments;
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            appointments = _dataService.GetAppointments();
        }
        else
        {
            appointments = _dataService.SearchAppointments(searchTerm);
        }

        appointments = appointments.OrderBy(a => a.Date).ToList();

        var patients = _dataService.GetPatients().ToDictionary(p => p.Id);
        var doctors = _dataService.GetDoctors().ToDictionary(d => d.Id);

        var viewModel = appointments.Select(a => new AppointmentListItemViewModel
        {
            Id = a.Id,
            PatientName = patients.TryGetValue(a.PatientId, out var p) ? p.FullName : "Unknown",
            DoctorName = doctors.TryGetValue(a.DoctorId, out var d) ? d.FullName : "Unknown",
            Date = a.Date,
            Reason = a.Reason,
            Status = a.Status
        });

        ViewBag.SearchTerm = searchTerm;
        return View(viewModel);
    }

    public IActionResult Create()
    {
        ViewBag.Patients = _dataService.GetPatients().ToList();
        ViewBag.Doctors = _dataService.GetDoctors().ToList();
        return View(new Appointment { Date = DateTime.Now.AddHours(1) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Appointment appointment)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Patients = _dataService.GetPatients().ToList();
            ViewBag.Doctors = _dataService.GetDoctors().ToList();
            return View(appointment);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;

        try
        {
            var created = _dataService.AddAppointment(appointment);

            _dataService.AddAuditLog(new AuditLog
            {
                UserId = currentUserId,
                UserName = User.Identity?.Name ?? "Unknown",
                Action = AuditAction.Create,
                EntityType = "Appointment",
                EntityId = created.Id,
                Details = $"Created appointment for {created.Date:yyyy-MM-dd HH:mm}",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            // Notify manager if approval is required
            var managers = _dataService.GetUsers().Where(u => u.Role == UserRole.Manager && u.IsActive);
            foreach (var manager in managers)
            {
                _dataService.AddNotification(new Notification
                {
                    UserId = manager.Id,
                    Title = "Appointment Approval Required",
                    Message = $"New appointment created and requires approval",
                    Type = NotificationType.ApprovalRequired,
                    ActionUrl = $"/Appointments/Edit/{created.Id}",
                    RelatedEntityId = created.Id,
                    RelatedEntityType = "Appointment"
                });
            }

            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Patients = _dataService.GetPatients().ToList();
            ViewBag.Doctors = _dataService.GetDoctors().ToList();
            return View(appointment);
        }
    }

    public IActionResult Edit(int id)
    {
        var appointment = _dataService.GetAppointment(id);
        if (appointment == null)
            return NotFound();

        ViewBag.Patients = _dataService.GetPatients().ToList();
        ViewBag.Doctors = _dataService.GetDoctors().ToList();
        return View(appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Appointment appointment)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Patients = _dataService.GetPatients().ToList();
            ViewBag.Doctors = _dataService.GetDoctors().ToList();
            return View(appointment);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Staff";

        var existing = _dataService.GetAppointment(id);
        if (existing == null)
            return NotFound();

        var updated = _dataService.UpdateAppointment(id, appointment);
        if (updated == null)
            return NotFound();

        var action = existing.Status != appointment.Status && appointment.Status == AppointmentStatus.Approved
            ? AuditAction.Approve
            : AuditAction.Update;

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = action,
            EntityType = "Appointment",
            EntityId = id,
            Details = $"Updated appointment: Status changed to {appointment.Status}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "ManagerOrAdmin")]
    public IActionResult Delete(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;

        var appointment = _dataService.GetAppointment(id);
        if (appointment == null)
            return NotFound();

        _dataService.DeleteAppointment(id);

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Delete,
            EntityType = "Appointment",
            EntityId = id,
            Details = $"Deleted appointment",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }
}





