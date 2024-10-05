using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace InventoryHubUI.Services.APITokens
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        //public async Task<HttpResponseMessage> GetDataFromApiAsync(string endpoint)
        //{
        //    var token = _httpContextAccessor.HttpContext.Session.GetString("JWTToken");
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        throw new UnauthorizedAccessException("Token is missing");
        //    }

        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    return await _httpClient.GetAsync(endpoint);
        //}

        //public async Task<HttpResponseMessage> PostDataToApiAsync(string endpoint, HttpContent content)
        //{
        //    var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
        //    if (string.IsNullOrEmpty(token))
        //    {
        //        throw new UnauthorizedAccessException("Token is missing");
        //    }

        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    return await _httpClient.PostAsync(endpoint, content);
        //}
    }


}
