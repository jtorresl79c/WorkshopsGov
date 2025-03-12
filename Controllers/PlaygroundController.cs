using Microsoft.AspNetCore.Mvc;


namespace WorkshopsGov.Controllers;

public class PlaygroundController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}