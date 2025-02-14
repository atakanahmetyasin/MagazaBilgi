using Microsoft.AspNetCore.Mvc;

namespace IadeFormu.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Anasayfa()
        {
            var magaza = HttpContext.Session.GetString("Magaza");
            ViewBag.Magaza = magaza; // veya ViewData["Magaza"] = magaza;
            return View();
        }
    }
}
