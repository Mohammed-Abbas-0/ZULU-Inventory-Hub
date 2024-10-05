using InventoryHubUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryHubUI.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(string token=null,string refreshtoken=null,string UserId=null,string Email=null)
        {
            if(!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshtoken) && !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(Email))
            {
                HttpContext.Session.SetString("JWTToken", token); // لو الـ API بيبعت JWT Token
                HttpContext.Session.SetString("RefreshToken", refreshtoken); // لو الـ API بيبعت JWT Token

                // إعداد ملفات تعريف الارتباط للمصادقة
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Email),
                    new Claim(ClaimTypes.NameIdentifier, UserId) // تأكد من حصولك على userId من استجابة API
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index");
            }

            string sessionToken = HttpContext.Session.GetString("RefreshToken");
            // تحقق مما إذا كان المستخدم قد سجل الدخول
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(sessionToken))
            {
                // إذا لم يكن المستخدم مسجلاً، قم بإعادة توجيهه إلى صفحة تسجيل الدخول
                return RedirectToAction("Login", "Account");
            }
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
