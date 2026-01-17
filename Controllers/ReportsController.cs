using System.Security.Claims;
using System.Text.Json;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskStatusModel = HospitalManagementSystem.Models.TaskStatus;

namespace HospitalManagementSystem.Controllers;

[Authorize(Policy = "ManagerOrAdmin")]
public class ReportsController : Controller
{
    private readonly HospitalDataService _dataService;

    public ReportsController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var reports = _dataService.GetReports();
        return View(reports);
    }

    public IActionResult Generate()
    {
        return View(new ReportViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Generate(ReportViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;

        var reportData = new Dictionary<string, object>();
        var details = new List<Dictionary<string, object>>();

        switch (model.ReportType.ToLower())
        {
            case "appointments":
                var appointments = _dataService.GetAppointments();
                if (model.StartDate.HasValue)
                    appointments = appointments.Where(a => a.Date >= model.StartDate.Value);
                if (model.EndDate.HasValue)
                    appointments = appointments.Where(a => a.Date <= model.EndDate.Value);

                reportData["Total"] = appointments.Count();
                reportData["Pending"] = appointments.Count(a => a.Status == AppointmentStatus.Pending);
                reportData["Approved"] = appointments.Count(a => a.Status == AppointmentStatus.Approved);
                reportData["Cancelled"] = appointments.Count(a => a.Status == AppointmentStatus.Cancelled);

                var patients = _dataService.GetPatients().ToDictionary(p => p.Id);
                var doctors = _dataService.GetDoctors().ToDictionary(d => d.Id);

                foreach (var apt in appointments)
                {
                    details.Add(new Dictionary<string, object>
                    {
                        ["Id"] = apt.Id,
                        ["Patient"] = patients.TryGetValue(apt.PatientId, out var p) ? p.FullName : "Unknown",
                        ["Doctor"] = doctors.TryGetValue(apt.DoctorId, out var d) ? d.FullName : "Unknown",
                        ["Date"] = apt.Date.ToString("yyyy-MM-dd HH:mm"),
                        ["Status"] = apt.Status.ToString(),
                        ["Reason"] = apt.Reason
                    });
                }
                break;

            case "patients":
                var patientsList = _dataService.GetPatients();
                reportData["Total"] = patientsList.Count();
                reportData["Male"] = patientsList.Count(p => p.Gender == "Male");
                reportData["Female"] = patientsList.Count(p => p.Gender == "Female");

                foreach (var patient in patientsList)
                {
                    details.Add(new Dictionary<string, object>
                    {
                        ["Id"] = patient.Id,
                        ["Name"] = patient.FullName,
                        ["Age"] = patient.Age,
                        ["Gender"] = patient.Gender,
                        ["Phone"] = patient.Phone,
                        ["Address"] = patient.Address
                    });
                }
                break;

            case "tasks":
                var tasks = _dataService.GetTasks();
                if (model.StartDate.HasValue)
                    tasks = tasks.Where(t => t.CreatedAt >= model.StartDate.Value);
                if (model.EndDate.HasValue)
                    tasks = tasks.Where(t => t.CreatedAt <= model.EndDate.Value);

                reportData["Total"] = tasks.Count();
                reportData["Pending"] = tasks.Count(t => t.Status == TaskStatusModel.Pending);
                reportData["InProgress"] = tasks.Count(t => t.Status == TaskStatusModel.InProgress);
                reportData["Completed"] = tasks.Count(t => t.Status == TaskStatusModel.Completed);

                var users = _dataService.GetUsers().ToDictionary(u => u.Id);

                foreach (var task in tasks)
                {
                    details.Add(new Dictionary<string, object>
                    {
                        ["Id"] = task.Id,
                        ["Title"] = task.Title,
                        ["AssignedTo"] = users.TryGetValue(task.AssignedToUserId, out var u) ? u.FullName : "Unknown",
                        ["Status"] = task.Status.ToString(),
                        ["Priority"] = task.Priority.ToString(),
                        ["DueDate"] = task.DueDate?.ToString("yyyy-MM-dd") ?? "N/A",
                        ["CreatedAt"] = task.CreatedAt.ToString("yyyy-MM-dd HH:mm")
                    });
                }
                break;

            case "users":
                var usersList = _dataService.GetUsers();
                reportData["Total"] = usersList.Count();
                reportData["Active"] = usersList.Count(u => u.IsActive);
                reportData["Administrators"] = usersList.Count(u => u.Role == UserRole.Administrator);
                reportData["Managers"] = usersList.Count(u => u.Role == UserRole.Manager);
                reportData["Staff"] = usersList.Count(u => u.Role == UserRole.Staff);

                foreach (var user in usersList)
                {
                    details.Add(new Dictionary<string, object>
                    {
                        ["Id"] = user.Id,
                        ["UserName"] = user.UserName,
                        ["FullName"] = user.FullName,
                        ["Email"] = user.Email,
                        ["Role"] = user.Role.ToString(),
                        ["IsActive"] = user.IsActive,
                        ["CreatedAt"] = user.CreatedAt.ToString("yyyy-MM-dd")
                    });
                }
                break;
        }

        model.Summary = reportData;
        model.Details = details;

        var report = new Report
        {
            Title = $"{model.ReportType} Report - {DateTime.Now:yyyy-MM-dd HH:mm}",
            ReportType = model.ReportType,
            GeneratedByUserId = currentUserId,
            Data = JsonSerializer.Serialize(model)
        };

        _dataService.AddReport(report);

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.View,
            EntityType = "Report",
            EntityId = report.Id,
            Details = $"Generated {model.ReportType} report",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return View("ViewReport", model);
    }

    public IActionResult ViewReport(int id)
    {
        var report = _dataService.GetReport(id);
        if (report == null)
            return NotFound();

        var model = JsonSerializer.Deserialize<ReportViewModel>(report.Data);
        if (model == null)
            return NotFound();

        return View(model);
    }
}

