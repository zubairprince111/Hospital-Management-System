using System.Security.Claims;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Authorize(Policy = "StaffOrAbove")]
public class NotificationsController : Controller
{
    private readonly HospitalDataService _dataService;

    public NotificationsController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;

        var notifications = _dataService.GetNotificationsByUser(userId);
        return View(notifications);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkAsRead(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;

        var notification = _dataService.GetNotification(id);
        if (notification == null || notification.UserId != userId)
        {
            return NotFound();
        }

        _dataService.MarkNotificationAsRead(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkAllAsRead()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;

        _dataService.MarkAllNotificationsAsRead(userId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;

        var notification = _dataService.GetNotification(id);
        if (notification == null || notification.UserId != userId)
        {
            return NotFound();
        }

        _dataService.DeleteNotification(id);
        return RedirectToAction(nameof(Index));
    }
}

