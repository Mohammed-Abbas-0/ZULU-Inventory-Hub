using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Helper.FilebaseStorage
{
    public class StorageService : IStorageService
    {
        private readonly FirebaseStorage _firebaseStorage;

        public StorageService(IConfiguration configuration)
        {
            var firebaseStorageUrl = configuration["FirebaseSettings:StorageBucket"];
            _firebaseStorage = new FirebaseStorage(firebaseStorageUrl);
        }
        public async Task<Tuple<string, bool>> Upload(int productId,IFormFile imagePath,string StorageFileName)
        {
            try
            {
                // تعيين مسار الملف المرفوع
                var fileName = Path.GetFileName(imagePath.FileName);
                var stream = imagePath.OpenReadStream();

                //رفع الملف إلى Firebase Storage
                var task = _firebaseStorage
                    .Child("uploads")
                    .Child(StorageFileName)                  // مجلد المنتجات
                    .Child(productId.ToString())       // رقم المنتج كجزء من المسار
                    .Child(fileName)
                    .PutAsync(stream);

                //انتظار انتهاء الرفع
                var downloadUrl = await task;
                return Tuple.Create(downloadUrl, true);
            }
            catch (Exception ex)
            {
                return Tuple.Create(ex.Message, false);
            }
        }
        public async Task<bool> DeleteAsync(string imagePath)
        {
            try
            {
                // استخراج المسار من الـ URL
                Uri uri = new Uri(imagePath);
                string fullPath = uri.AbsolutePath;  // /v0/b/inventoryhub-6d5a2.appspot.com/o/uploads%2FScreenshot%202024-09-17%20223639.png
                string decodedPath = Uri.UnescapeDataString(fullPath);  // يفك التشفير
                string imageUrl = decodedPath.Replace("/v0/b/inventoryhub-6d5a2.appspot.com/o/", "");  // يزيل الجزء غير المرغوب فيه

                // التأكد من أن المسار لا يحتوي على مشاكل
                if (string.IsNullOrEmpty(imageUrl) || imageUrl.Contains("//"))
                {
                    throw new ArgumentException("Invalid image path.");
                }

                var storageReference = _firebaseStorage.Child(imageUrl);

                // التحقق مما إذا كان الملف موجودًا قبل الحذف
                var exists = await storageReference.GetDownloadUrlAsync();
                if (exists != null)
                {
                    // إذا كان الملف موجودًا، قم بحذفه
                    await storageReference.DeleteAsync();
                    Console.WriteLine("Image deleted successfully.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Image not found.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
