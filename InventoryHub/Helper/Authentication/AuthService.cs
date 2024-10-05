
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using InventoryHub.Models;
using InventoryHub.ModelViews;

namespace InventoryHub.Helper.Authentication
{
    public class AuthService : IAuthService
    {
        private const int refreshTokenExpireDuration = 1;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _identityRole;
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> identityRole, IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _identityRole = identityRole;
            _jwt =  jwt.Value;
        }
     
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "Email is already registered!" };

            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Message = "Username is already registered!" };

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            #region ERRORS
            if (!result.Succeeded)
            {
                StringBuilder errors = new();

                foreach (var error in result.Errors)
                    errors.Append($"{error.Description},");

                return new AuthModel { Message = errors.ToString() };
            }
            #endregion
            if (!await _identityRole.RoleExistsAsync("User"))
                await _identityRole.CreateAsync(new IdentityRole("User"));

            await _userManager.AddToRoleAsync(user, "User");


            var jwtSecurityToken = await Create_JWT_TokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            return new AuthModel
            {
                Email = user.Email,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = jwtSecurityToken,
                Username = user.UserName,
                RefreshToken = refreshToken.Token
            };
        }

        #region  LOGIN
        public async Task<AuthModel> LoginAsync(TokenRequestModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new AuthModel { IsAuthenticated = false, Message = "User not found" };
            }

            // إذا كان المستخدم قادم من تسجيل دخول عبر Google، تخطى التحقق من كلمة المرور
            if (model.Password == null)
            {
                // Access Token
                var jwt = await Create_JWT_TokenAsync(user);
                var authModel = new AuthModel
                {
                    IsAuthenticated = true,
                    Token = jwt,
                    Username = user.UserName,
                    UserId = user.Id
                };

                // التعامل مع الـ RefreshToken
                var activeToken = user.RefreshTokens.FirstOrDefault(rt => !rt.IsExpired);
                if (activeToken != null && activeToken.ExpiresOn > DateTime.UtcNow)
                {
                    authModel.RefreshToken = activeToken.Token;
                    authModel.RefreshTokenExpiration = activeToken.ExpiresOn;
                }
                else
                {
                    var refreshToken = GenerateRefreshToken();
                    authModel.RefreshToken = refreshToken.Token;
                    authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;

                    user.RefreshTokens.Add(refreshToken);
                    await _userManager.UpdateAsync(user);
                }

                return authModel;
            }

            // تحقق من كلمة المرور إذا كانت موجودة
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthModel { IsAuthenticated = false, Message = "Incorrect Username or Password" };
            }

            // Access Token
            var jwtToken = await Create_JWT_TokenAsync(user);
            var authResult = new AuthModel
            {
                IsAuthenticated = true,
                Token = jwtToken,
                Username = user.UserName,
                UserId = user.Id
            };

            // التعامل مع الـ RefreshToken
            var existingToken = user.RefreshTokens.FirstOrDefault(rt => !rt.IsExpired);
            if (existingToken != null && existingToken.ExpiresOn > DateTime.UtcNow)
            {
                authResult.RefreshToken = existingToken.Token;
                authResult.RefreshTokenExpiration = existingToken.ExpiresOn;
            }
            else
            {
                var newRefreshToken = GenerateRefreshToken();
                authResult.RefreshToken = newRefreshToken.Token;
                authResult.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authResult;
        }
        #endregion

        #region CREATE   JWT   TOKEN
        private async Task<string> Create_JWT_TokenAsync(ApplicationUser user)
        {

            var getUserClaims = await _userManager.GetClaimsAsync(user);
            var getUserRoles = await _userManager.GetRolesAsync(user);

            List<Claim> roles = new();
            foreach (var role in getUserRoles)
                roles.Add(new Claim("role",role));

            IEnumerable<Claim> claims = new[] { 
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Name,user.FirstName+user.LastName),
                new Claim("UID",user.Id),
                
            }
            .Union(getUserClaims)
            .Union(roles);


            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwt.Key));
            SigningCredentials signingCredentials = new(key,SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new(
                issuer:_jwt.ValidIssuer,
                audience:_jwt.ValidAudience,
                expires:DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                claims:claims,
                signingCredentials:signingCredentials
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return jwt;
        }

        #endregion

        #region Generate  Refresh Token
        private RefreshToken GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider(); // توليد ارقام عشوائية عالية الامان

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(refreshTokenExpireDuration),
                CreatedOn = DateTime.UtcNow
            };
        }

        #endregion

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {

            AuthModel authModel = new();
            ApplicationUser user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthModel(){ Message = "Email Or Password is incorrect."};
            
            string jwtToken = await Create_JWT_TokenAsync(user);
            IList<string> Roles = await _userManager.GetRolesAsync(user);

            authModel.Token = jwtToken;
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
           // authModel.ExpiresOn = jwtToken.ValidTo;
            authModel.IsAuthenticated = true;
            authModel.Roles = Roles.ToList();

            if(user.RefreshTokens.Any(idx=>!idx.IsExpired))
            {
                RefreshToken activeToken = user.RefreshTokens.FirstOrDefault(idx=>!idx.IsExpired);
                authModel.RefreshToken = activeToken.Token;
                authModel.RefreshTokenExpiration = activeToken.ExpiresOn;
            }
            else
            {
                RefreshToken refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user is null || !await _identityRole.RoleExistsAsync(model.Role))
                return "Invalid user ID or Role";

            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "User already assigned to this role";

            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Sonething went wrong";
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, // لا نحتاج إلى التحقق من الـ Audience
                ValidateIssuer = false,   // لا نحتاج إلى التحقق من الـ Issuer
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your_Secret_Key_Here")),
                ValidateLifetime = false // نسمح بانتهاء صلاحية التوكن
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtToken = securityToken as JwtSecurityToken;
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public bool VerifyToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwt.ValidIssuer,
                    ValidAudience = _jwt.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key))
                };

                handler.ValidateToken(token, validationParameters, out var validatedToken);
                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                return false;
            }

            var refreshToken = user.RefreshTokens
                .FirstOrDefault(t => t.Token == token);

            if (refreshToken == null)
            {
                return false;
            }

            user.RefreshTokens.Remove(refreshToken);
            await _userManager.UpdateAsync(user);

            return true;
        }



        public async Task<AuthModel> RefreshTokenAsync(RefreshTokenRequestModel refreshTokenRequest)
        {
            var user = await _userManager.Users.Include(u => u.RefreshTokens).SingleOrDefaultAsync(u => u.Id == refreshTokenRequest.UserId);
            if (user is null)
                return new AuthModel { Message = "Invalid User.", IsAuthenticated = false };

            // حذف التوكنات المنتهية
            var expiredTokens = user.RefreshTokens.Where(t => t.ExpiresOn < DateTime.UtcNow).ToList();
            foreach (var expiredToken in expiredTokens)
            {
                user.RefreshTokens.Remove(expiredToken);
            }

            var token = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshTokenRequest.RefreshToken);
            if (token == null || token.ExpiresOn < DateTime.UtcNow)
                return new AuthModel { Message = "Invalid or expired refresh token.", IsAuthenticated = false };

            // تجديد الـ Access Token
            var newAccessToken = await Create_JWT_TokenAsync(user);

            // توليد Refresh Token جديد
            var newRefreshToken = GenerateRefreshToken();
            token.Token = newRefreshToken.Token;
            token.ExpiresOn = newRefreshToken.ExpiresOn;

            // حفظ التغييرات في قاعدة البيانات
            await _userManager.UpdateAsync(user);

            return new AuthModel
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                Message = "Token refreshed successfully.",
                IsAuthenticated = true
            };
        }



    }

}
