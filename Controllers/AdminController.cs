using System.Security.Claims;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

public class AdminController : Controller
{
    private readonly HospitalDataService _dataService;

    public AdminController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet("/Admin/Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("/Admin/Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminUser model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _dataService.GetUserByUsername(model.UserName);
        if (user != null && user.Password == model.Password && user.IsActive)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Log login
            _dataService.AddAuditLog(new AuditLog
            {
                UserId = user.Id,
                UserName = user.UserName,
                Action = AuditAction.Login,
                EntityType = "User",
                EntityId = user.Id,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            return RedirectToAction("Dashboard");
        }

        ModelState.AddModelError(string.Empty, "Invalid username or password.");
        return View(model);
    }

    [Authorize]
    [HttpPost("/Admin/Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            var user = _dataService.GetUser(userId);
            if (user != null)
            {
                _dataService.AddAuditLog(new AuditLog
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Action = AuditAction.Logout,
                    EntityType = "User",
                    EntityId = user.Id,
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
                });
            }
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [Authorize]
    [HttpGet("/Admin/Dashboard")]
    public IActionResult Dashboard()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        var userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Staff";

        var appointments = _dataService
            .GetAppointments()
            .Where(a => a.Date >= DateTime.Today)
            .OrderBy(a => a.Date)
            .Take(10)
            .ToList();

        var model = new DashboardViewModel
        {
            TotalPatients = _dataService.GetPatients().Count(),
            TotalDoctors = _dataService.GetDoctors().Count(),
            TotalAppointments = _dataService.GetAppointments().Count(),
            UpcomingAppointments = appointments
        };

        // Pass patient and doctor dictionaries for display
        ViewBag.Patients = _dataService.GetPatients().ToDictionary(p => p.Id, p => p.FullName);
        ViewBag.Doctors = _dataService.GetDoctors().ToDictionary(d => d.Id, d => d.FullName);
        ViewBag.UserId = userId;
        ViewBag.UserRole = userRole;
        ViewBag.UnreadNotifications = _dataService.GetNotificationsByUser(userId).Count();
        ViewBag.PendingAppointmentsCount = _dataService.GetAppointments().Count(a => a.Status == AppointmentStatus.Pending);

        return View(model);
    }
}





