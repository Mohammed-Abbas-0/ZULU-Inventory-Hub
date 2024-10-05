using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Helper.HelperMethods
{
    public interface IHelperMethod
    {
         IFormFile ConvertToFormFile(string imagePath);
    }
}
