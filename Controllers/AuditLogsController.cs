using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[Authorize(Policy = "ManagerOrAdmin")]
public class AuditLogsController : Controller
{
    private readonly HospitalDataService _dataService;

    public AuditLogsController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index(string entityType = "", int? entityId = null)
    {
        IEnumerable<AuditLog> logs;

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            logs = _dataService.GetAuditLogsByEntity(entityType, entityId);
        }
        else
        {
            logs = _dataService.GetAuditLogs();
        }

        ViewBag.EntityType = entityType;
        ViewBag.EntityId = entityId;
        ViewBag.Users = _dataService.GetUsers().ToDictionary(u => u.Id, u => u.FullName);

        return View(logs);
    }
}

