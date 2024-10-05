using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InventoryHub.Helper.ExternalLogin
{
    public interface IExternalLogin
    {
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string ProviderName,string redirectUrl);
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
        Task<SignInResult> ExternalLoginSignInAsync(string LoginProvider, string ProviderKey,bool isPersistent= false);
        Task<bool> AddUser(ExternalLoginInfo Principal);
        Task<bool> CheckUser(string email, ExternalLoginInfo info);
       
    }
}
