using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ModelViews
{
    public class RefreshTokenRequestModel
    {
        public string UserId { get; set; }
        public string Token { get; set; } 
        public string RefreshToken { get; set; } 
    }
}
