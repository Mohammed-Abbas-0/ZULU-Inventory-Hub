using InventoryHub.Helper.Authentication;
using InventoryHub.Helper.ExternalLogin;
using InventoryHub.Models;
using InventoryHub.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryHub.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IExternalLogin _externalLogin;
        public AccountController(IAuthService authService, IExternalLogin externalLogin)
        {
            _authService = authService;
            _externalLogin = externalLogin;
        }


        #region   REGISTER

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        #endregion

        #region  LOGIN

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(model);
            
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }

            return Ok(new
            {
                FirstName=result.FirstName,
                LastName=result.LastName,
                UserId = result.UserId,
                token = result.Token,
                refreshToken = result.RefreshToken,
                refreshTokenExpiration = result.RefreshTokenExpiration,
                message = "Login successful"
            });
        }

        #endregion


        #region  LOGOUT
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Token))
            {
                return BadRequest("Token is required.");
            }
            model.Token = model.Token.Replace(" ","+");
            // قم بإبطال التوكن
            var result = await _authService.RevokeTokenAsync(model.Token);

            if (result)
            {
                return Ok(new { Message = "Logout successful" });
            }

            return BadRequest("Invalid or inactive token.");
        }

        #endregion

        #region Refresh Token
        [Authorize]
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Token))
                return BadRequest("Invalid Token");

            var authModel = await _authService.RefreshTokenAsync(model);
            if (!authModel.IsAuthenticated)
                return BadRequest(authModel.Message);

            return Ok(authModel);
        }

        #endregion



        #region   GOOGLE


        [HttpGet("GoogleLogin")]
        public IActionResult GoogleLogin(string returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new { returnUrl });
            var properties = _externalLogin.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return BadRequest(new { message = $"Error from Google: {remoteError}" });
            }

            var info = await _externalLogin.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest(new { message = "Error fetching Google login info" });
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return BadRequest(new { message = "Email not provided by Google" });
            }

            // تحقق من وجود المستخدم وربطه بحساب Google
            var userCreationResult = await _externalLogin.CheckUser(email, info);
            if (!userCreationResult)
            {
                return BadRequest(new { message = "Failed to create or link user" });
            }

            // إصدار التوكن للمستخدم
            TokenRequestModel model = new() { Email = email };
            var loginResult = await _authService.LoginAsync(model);

            if (!loginResult.IsAuthenticated)
            {
                return BadRequest(loginResult.Message);
            }

            // إذا كان الـ returnUrl موجودًا، أعد التوجيه إليه مع تضمين التوكن
            if (!string.IsNullOrEmpty(returnUrl))
            {
                // إضافة التوكن إلى الـ returnUrl

                var redirectUrl = $"{returnUrl}?token={loginResult.Token}&refreshtoken={loginResult.RefreshToken}&UserId={loginResult.UserId}&Email={email}";
                return Redirect(redirectUrl);
            }

            // افتراضيًا، إعادة التوجيه إلى صفحة معينة إذا لم يكن هناك returnUrl
            return Ok(loginResult);
        }




        #endregion
    }


}

