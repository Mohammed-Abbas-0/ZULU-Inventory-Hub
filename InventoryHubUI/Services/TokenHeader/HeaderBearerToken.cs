using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace InventoryHubUI.Services.TokenHeader
{
    public class HeaderBearerToken : ControllerBase, IHeaderBearerToken
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        public HeaderBearerToken(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }
        public async Task<string> HeaderTokenRequest()
        {
            // الحصول على التوكن من الجلسة
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
            var refreshToken = _httpContextAccessor.HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            {

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return "";
            }

            return token;
        }
    }
}
