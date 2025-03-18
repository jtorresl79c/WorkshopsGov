using Microsoft.AspNetCore.Mvc;


namespace WorkshopsGov.Controllers;

public class PlaygroundController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("api/[controller]/Get")]
    public IEnumerable<string> Get()
    {
        return new string[] { "Get Playground API 1", "Get Playground API 2" };
    }
    [HttpPost]
    [Route("api/[controller]/Create")]
    public IEnumerable<string> Create()
    {
        return new string[] { "Post Playground API 1", "Post Playground API 2" };
    }
}