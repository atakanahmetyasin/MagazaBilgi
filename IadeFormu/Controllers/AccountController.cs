using System.Linq;
using IadeFormu.Data;
using IadeFormu.Models;
using Microsoft.AspNetCore.Mvc;

namespace IadeFormu.Controllers
{
    public class AccountController : Controller
    {
        private readonly MagazaBilgiDbContext _context;

        public AccountController(MagazaBilgiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u =>
                u.KullaniciAdi == model.KullaniciAdi && u.Sifre == model.Sifre
            );

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                return View(model);
            }

            // Kullanıcıyı session'a ekleyin (veya authentication mekanizması kullanın)
            HttpContext.Session.SetString("Magaza", user.Magaza);

            return RedirectToAction("Anasayfa", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
