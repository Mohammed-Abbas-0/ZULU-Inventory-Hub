using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Helper.HelperMethods
{
    public class HelperMethod : IHelperMethod
    {
        public IFormFile ConvertToFormFile(string imagePath)
        {
            // التحقق من أن المسار صحيح والملف موجود
            var fileName = Path.GetFileName(imagePath);
            var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

            // تحويل Stream إلى IFormFile
            var formFile = new FormFile(fileStream, 0, fileStream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg" // أو نوع المحتوى المناسب للصورة
            };

            return formFile;
        }

    }
}
