﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Models
{
    [Owned] // تم إستخدامه في كلاس تاني 
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public DateTime CreatedOn { get; set; }
        //public DateTime? RevokedOn { get; set; }
        //public bool IsActive => !IsExpired;
    }
}
