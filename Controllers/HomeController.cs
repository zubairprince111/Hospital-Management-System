using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly HospitalDataService _dataService;

    public HomeController(HospitalDataService dataService)
    {
        _dataService = dataService;
    }

    public IActionResult Index()
    {
        // If user is authenticated, redirect to admin dashboard
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        // For unauthenticated users, show public landing page
        ViewBag.Doctors = _dataService.GetDoctors().ToList();
        return View();
    }
}





