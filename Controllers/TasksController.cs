using System.Security.Claims;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskModel = HospitalManagementSystem.Models.Task;

namespace HospitalManagementSystem.Controllers;

[Authorize(Policy = "StaffOrAbove")]
public class TasksController : Controller
{
    private readonly HospitalDataService _dataService;

    public TasksController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Staff";

        IEnumerable<TaskModel> tasks;
        if (userRole == "Administrator" || userRole == "Manager")
        {
            tasks = _dataService.GetTasks();
        }
        else
        {
            tasks = _dataService.GetTasksByUser(userId);
        }

        var usersList = _dataService.GetUsers().Where(u => u.IsActive).ToList();
        ViewBag.Users = usersList.ToDictionary(u => u.Id, u => u.FullName);
        return View(tasks);
    }

    public IActionResult Create()
    {
        ViewBag.Users = _dataService.GetUsers().Where(u => u.IsActive).ToList();
        return View(new TaskModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TaskModel task)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Users = _dataService.GetUsers().Where(u => u.IsActive).ToList();
            return View(task);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;

        task.AssignedByUserId = currentUserId;
        var created = _dataService.AddTask(task);

        // Create notification for assigned user
        var assignedUser = _dataService.GetUser(task.AssignedToUserId);
        if (assignedUser != null)
        {
            _dataService.AddNotification(new Notification
            {
                UserId = task.AssignedToUserId,
                Title = "New Task Assigned",
                Message = $"You have been assigned a new task: {task.Title}",
                Type = NotificationType.TaskAssignment,
                ActionUrl = $"/Tasks/Edit/{created.Id}"
            });
        }

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Create,
            EntityType = "Task",
            EntityId = created.Id,
            Details = $"Created task: {task.Title}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var task = _dataService.GetTask(id);
        if (task == null)
            return NotFound();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var userId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Staff";

        // Staff can only edit their own tasks
        if (userRole == "Staff" && task.AssignedToUserId != userId)
        {
            return Forbid();
        }

        ViewBag.Users = _dataService.GetUsers().Where(u => u.IsActive).ToList();
        return View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TaskModel task)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Users = _dataService.GetUsers().Where(u => u.IsActive).ToList();
            return View(task);
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var currentUserId = userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid) ? uid : 0;

        var existing = _dataService.GetTask(id);
        if (existing == null)
            return NotFound();

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Staff";
        if (userRole == "Staff" && existing.AssignedToUserId != currentUserId)
        {
            return Forbid();
        }

        var updated = _dataService.UpdateTask(id, task);
        if (updated == null)
            return NotFound();

        // Notify if task was reassigned
        if (existing.AssignedToUserId != task.AssignedToUserId)
        {
            var assignedUser = _dataService.GetUser(task.AssignedToUserId);
            if (assignedUser != null)
            {
                _dataService.AddNotification(new Notification
                {
                    UserId = task.AssignedToUserId,
                    Title = "Task Reassigned",
                    Message = $"Task '{task.Title}' has been reassigned to you",
                    Type = NotificationType.TaskAssignment,
                    ActionUrl = $"/Tasks/Edit/{id}"
                });
            }
        }

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Update,
            EntityType = "Task",
            EntityId = id,
            Details = $"Updated task: {task.Title}",
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
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Staff";

        var task = _dataService.GetTask(id);
        if (task == null)
            return NotFound();

        if (userRole != "Administrator" && userRole != "Manager")
        {
            return Forbid();
        }

        _dataService.DeleteTask(id);

        _dataService.AddAuditLog(new AuditLog
        {
            UserId = currentUserId,
            UserName = User.Identity?.Name ?? "Unknown",
            Action = AuditAction.Delete,
            EntityType = "Task",
            EntityId = id,
            Details = $"Deleted task: {task.Title}",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        });

        return RedirectToAction(nameof(Index));
    }
}

