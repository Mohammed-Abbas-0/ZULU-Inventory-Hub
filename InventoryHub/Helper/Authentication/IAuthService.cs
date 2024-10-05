using InventoryHub.Models;
using InventoryHub.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Helper.Authentication
{
    public interface IAuthService
    {
        Task<AuthModel> LoginAsync(TokenRequestModel model);
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel token);
        Task<string> AddRoleAsync(AddRoleModel roleModel);
        Task<AuthModel> RefreshTokenAsync(RefreshTokenRequestModel model);
        bool VerifyToken(string token);
        Task<bool> RevokeTokenAsync(string token);
    }
}
