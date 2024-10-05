using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Helper.FilebaseStorage
{
    public interface IStorageService
    {
        Task<Tuple<string, bool>> Upload(int productId,IFormFile formFile, string StorageFileName);
        Task<bool> DeleteAsync(string imagePath);
    }
}
