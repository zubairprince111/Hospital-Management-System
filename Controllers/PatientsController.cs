using System.Security.Claims;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Authorize(Policy = "StaffOrAbove")]
public class PatientsController : Controller
{
    private readonly HospitalDataService _dataService;

    public PatientsController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index(string searchTerm = "")
    {
        IEnumerable<Patient> patients;
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            patients = _dataService.GetPatients();
        }
        else
        {
            patients = _dataService.SearchPatients(searchTerm);
        }

        ViewBag.SearchTerm = searchTerm;
        return View(patients);
    }

    public IActionResult Create()
    {
        return View(new Patient());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Patient patient)
    {
        if (!ModelState.IsValid)
        {
            return View(patient);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;

        var created = _dataService.AddPatient(patient);

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Create,
            EntityType = "Patient",
            EntityId = created.Id,
            Details = $"Created patient: {patient.FullName}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var patient = _dataService.GetPatient(id);
        if (patient == null)
            return NotFound();

        return View(patient);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Patient patient)
    {
        if (!ModelState.IsValid)
        {
            return View(patient);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;

        var updated = _dataService.UpdatePatient(id, patient);
        if (updated == null)
            return NotFound();

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Update,
            EntityType = "Patient",
            EntityId = id,
            Details = $"Updated patient: {patient.FullName}",
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

        var patient = _dataService.GetPatient(id);
        if (patient == null)
            return NotFound();

        _dataService.DeletePatient(id);

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Delete,
            EntityType = "Patient",
            EntityId = id,
            Details = $"Deleted patient: {patient.FullName}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }
}





