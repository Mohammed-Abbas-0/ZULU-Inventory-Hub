using InventoryHubUI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InventoryHubUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region   LOGIN
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {

            string token = HttpContext.Session.GetString("RefreshToken");
            // تحقق مما إذا كان المستخدم قد سجل الدخول
            if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(model);

            
            var response = await _httpClient.PostAsJsonAsync(API_EndPoint.LoGinENDPoint, model);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponseModel>(content);
                // هنا تخزن التوكن في Session أو كوكي
                HttpContext.Session.SetString("JWTToken", tokenResponse.Token); // لو الـ API بيبعت JWT Token
                HttpContext.Session.SetString("RefreshToken", tokenResponse.RefreshToken); // لو الـ API بيبعت JWT Token

                // إعداد ملفات تعريف الارتباط للمصادقة
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.NameIdentifier, tokenResponse.UserId) // تأكد من حصولك على userId من استجابة API
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
               

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        #endregion

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstname, string lastname, string email, string password, string phoneNumber)
        {
            var registerModel = new LoginModel
            {
                FirstName = firstname,
                LastName = lastname,
                UserName = firstname.Trim() + lastname.Trim(),
                Email = email,
                Password = password,
                PhoneNumber = phoneNumber,
            };

            // تحويل البيانات إلى JSON
            var jsonContent = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");

            // إرسال الطلب إلى API
            // تأكد من تعديل URL الخاص بـ API
            var response = await _httpClient.PostAsync(API_EndPoint.RegisterENDPoint, jsonContent); 

            if (response.IsSuccessStatusCode)
            {
                // return RedirectToAction("Login");
                return Json(new { success = true, message = "تم التسجيل بنجاح!" });

            }
            else
            {
                // في حال وجود خطأ
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var apiResponse = JsonConvert.DeserializeObject<List<ErrorDetail>>(errorContent);
                    var errors = apiResponse.FirstOrDefault();
                    return Json(new { success = false, message = string.Join(" ", errors.description) });

                }
                catch (Exception ex) {
                    return Json(new { success = false, message = errorContent });
                }
            }
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // الحصول على التوكن من الجلسة
            string refreshToken = HttpContext.Session.GetString("RefreshToken");
            string jwtToken = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(jwtToken))
            {
                return BadRequest(new { message = "No token found in session." });
            }

            // إعداد نموذج طلب تسجيل الخروج
            var logoutRequestModel = new LogoutRequestModel { Token = refreshToken };
            var jsonContent = new StringContent(JsonConvert.SerializeObject(logoutRequestModel), Encoding.UTF8, "application/json");

            // تعيين التوكن في رؤوس الطلب
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            // إرسال طلب تسجيل الخروج إلى الـ API
            var url = $"{API_EndPoint.LogoutENDPoint}";
            var response = await _httpClient.PostAsync(url, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { message = "Failed to logout. Please try again." });
            }

            // مسح الجلسة
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            await Task.Delay(500);
            // إعادة التوجيه إلى الصفحة الرئيسية بعد تسجيل الخروج
            return RedirectToAction("login", "Account");
        }


        public async Task<IActionResult> RefreshToken()
        {
            string token = HttpContext.Session.GetString("JWTToken");
            string refreshToken = HttpContext.Session.GetString("RefreshToken");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("login", "Account");
            }

            var refreshTokenRequestModel = new RefreshTokenRequestModel() { Token=token,RefreshToken=refreshToken,UserId=userId};

            var jsonContent = new StringContent(JsonConvert.SerializeObject(refreshTokenRequestModel), Encoding.UTF8, "application/json");

            var url = $"{API_EndPoint.RefreshTokenENDPoint}";
            var response = await _httpClient.PostAsync(url,jsonContent);
            response.EnsureSuccessStatusCode();
            if(response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tokenDetails =   JsonConvert.DeserializeObject<RefreshTokenRequestModel>(content);
                var newToken = tokenDetails.Token;
                var newRefreshToken = tokenDetails.RefreshToken;

                HttpContext.Session.SetString("JWTToken", newToken); // لو الـ API بيبعت JWT Token
                HttpContext.Session.SetString("RefreshToken", newRefreshToken); // لو الـ API بيبعت JWT Token
                return Json(new { newToken, newRefreshToken });
            }
            return Json(new { });
        }

        [HttpGet]
        public IActionResult UnauthorizedAccess()
        {
            return View();
        }


        public async Task<IActionResult> GoogleSignInAsync()
        {
            // تأكد من استخدام الـ URL الصحيح من إعدادات Google
            //string googleLoginUrl = "https://localhost:44324/api/Account/GoogleLogin/";
            //return Redirect(googleLoginUrl);
            string homePage= "https://localhost:44375/Home/Index";
            // بناء رابط الـ API مع الـ returnUrl
            string apiUrl = $"{API_EndPoint.AccountEndPoint}GoogleLogin?returnUrl={homePage}";

            return Redirect(apiUrl); // إعادة التوجيه إلى الـ API لبدء عملية Google login

        }

        



    }

    public class LoginModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class ErrorDetail
    {
        public string code { get; set; }
        public string description { get; set; }
    }
    public class TokenResponseModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenRequestModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
    public class LogoutRequestModel
    {
        public string Token { get; set; }
    }
    public class AuthModel
    {
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        //  public DateTime ExpiresOn { get; set; }
        //  [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
