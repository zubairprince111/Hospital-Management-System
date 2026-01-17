using System.Security.Claims;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Authorize(Policy = "AdministratorOnly")]
public class UsersController : Controller
{
    private readonly HospitalDataService _dataService;

    public UsersController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var users = _dataService.GetUsers();
        return View(users);
    }

    public IActionResult Create()
    {
        return View(new User());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;

        _dataService.AddUser(user);

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Create,
            EntityType = "User",
            EntityId = user.Id,
            Details = $"Created user: {user.UserName} with role: {user.Role}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var user = _dataService.GetUser(id);
        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, User user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;

        var updated = _dataService.UpdateUser(id, user);
        if (updated == null)
            return NotFound();

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Update,
            EntityType = "User",
            EntityId = id,
            Details = $"Updated user: {user.UserName}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;

        var user = _dataService.GetUser(id);
        if (user == null)
            return NotFound();

        _dataService.DeleteUser(id);

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Delete,
            EntityType = "User",
            EntityId = id,
            Details = $"Deleted user: {user.UserName}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }
}

