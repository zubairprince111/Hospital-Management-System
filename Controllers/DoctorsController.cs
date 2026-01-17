using System.Security.Claims;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Authorize(Policy = "StaffOrAbove")]
public class DoctorsController : Controller
{
    private readonly HospitalDataService _dataService;

    public DoctorsController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index(string searchTerm = "")
    {
        IEnumerable<Doctor> doctors;
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            doctors = _dataService.GetDoctors();
        }
        else
        {
            doctors = _dataService.SearchDoctors(searchTerm);
        }

        ViewBag.SearchTerm = searchTerm;
        return View(doctors);
    }

    public IActionResult Create()
    {
        return View(new Doctor());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Doctor doctor)
    {
        if (!ModelState.IsValid)
        {
            return View(doctor);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;

        var created = _dataService.AddDoctor(doctor);

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Create,
            EntityType = "Doctor",
            EntityId = created.Id,
            Details = $"Created doctor: {doctor.FullName}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var doctor = _dataService.GetDoctor(id);
        if (doctor == null)
            return NotFound();

        return View(doctor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Doctor doctor)
    {
        if (!ModelState.IsValid)
        {
            return View(doctor);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;

        var updated = _dataService.UpdateDoctor(id, doctor);
        if (updated == null)
            return NotFound();

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Update,
            EntityType = "Doctor",
            EntityId = id,
            Details = $"Updated doctor: {doctor.FullName}",
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

        var doctor = _dataService.GetDoctor(id);
        if (doctor == null)
            return NotFound();

        _dataService.DeleteDoctor(id);

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Delete,
            EntityType = "Doctor",
            EntityId = id,
            Details = $"Deleted doctor: {doctor.FullName}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }
}





