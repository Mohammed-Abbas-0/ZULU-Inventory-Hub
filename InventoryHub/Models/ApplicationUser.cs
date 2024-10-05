using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required,MaxLength(70)]
        public string FirstName { get; set; }

        [Required,MaxLength(70)]
        public string LastName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public List<RefreshToken>? RefreshTokens { get; set; }

    }
}
