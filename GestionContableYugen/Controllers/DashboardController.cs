using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewBag.Usuario = HttpContext.Session.GetString("Usuario");
        return View();
    }

}
