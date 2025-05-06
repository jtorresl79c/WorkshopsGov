using Microsoft.AspNetCore.Mvc;

namespace WorkshopsGov.Controllers;

public class ReportsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}