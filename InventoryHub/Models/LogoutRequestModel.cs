using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    public class LogoutRequestModel
    {
        [Required]
        public string Token { get; set; }
    }
}
