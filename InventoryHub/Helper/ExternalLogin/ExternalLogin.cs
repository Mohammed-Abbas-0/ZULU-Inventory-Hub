using InventoryHub.Helper.Authentication;
using InventoryHub.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryHub.Helper.ExternalLogin
{
    public class ExternalLogin : IExternalLogin
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExternalLogin(SignInManager<ApplicationUser> signInManager, IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _authService = authService;
            _userManager = userManager;
        }
        public AuthenticationProperties ConfigureExternalAuthenticationProperties(string ProviderName, string redirectUrl)
        {
            var properties =  _signInManager.ConfigureExternalAuthenticationProperties(ProviderName, redirectUrl);
            return properties;
        }


        public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
           var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
           return externalLoginInfo;
        }
     
        public async Task<SignInResult> ExternalLoginSignInAsync(string LoginProvider, string ProviderKey, bool isPersistent = false)
        {
            var signInResult = await _signInManager.ExternalLoginSignInAsync(LoginProvider, ProviderKey, isPersistent: false);
            if(signInResult.Succeeded)
                return signInResult;

            if (!signInResult.Succeeded)
                return null;
            return signInResult;

        }

        public async Task<bool> AddUser(ExternalLoginInfo info)
        {
            // إنشاء مستخدم جديد
            var user = new ApplicationUser
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return false;
            }

            // ربط حساب Google بالمستخدم
            result = await _userManager.AddLoginAsync(user, info);
            return result.Succeeded;
        }

        public async Task<bool> CheckUser(string email, ExternalLoginInfo info)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // إذا لم يكن المستخدم موجودًا، قم بإنشائه
                var userCreationResult = await AddUser(info);
                if (!userCreationResult)
                {
                    return false;
                }

                user = await _userManager.FindByEmailAsync(email); // إعادة جلب المستخدم بعد الإنشاء
            }

            // إذا كان المستخدم موجودًا، قم بربط حساب Google به
            var result = await _userManager.AddLoginAsync(user, info);
            // تحقق من حالة الخطأ لتحديد ما إذا كان الحساب مرتبطًا بالفعل
            if (result.Errors.Any(e => e.Code == "LoginAlreadyAssociated"))
            {
                // الحساب مرتبط بالفعل
                return true; // يمكنك تغيير هذا لتسجيل الحالة أو تقديم رسالة مناسبة للمستخدم
            }
            return result.Succeeded;
        }

    }
}
