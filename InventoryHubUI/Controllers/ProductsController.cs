using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using InventoryHubUI.DTO;
using InventoryHubUI.Services;
using InventoryHubUI.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Net.Http.Headers;
using InventoryHubUI.Services.HelperMethods;
using Microsoft.AspNetCore.Mvc.Rendering;
using InventoryHubUI.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using InventoryHubUI.Services.TokenHeader;

namespace InventoryHubUI.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ICategoryService _categoryService;
        private readonly IStoreService _storeService;
        private readonly IHeaderBearerToken _headerBearerToken;


        public ProductsController(HttpClient httpClient,ICategoryService categoryService, IStoreService storeService, IHeaderBearerToken headerBearerToken)
        {
            _httpClient = httpClient;
            _categoryService = categoryService;
            _storeService = storeService;
            _headerBearerToken = headerBearerToken;
        }
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var url = $"{API_EndPoint.ProductENDPoint}?pageNumber={pageNumber}&pageSize={pageSize}";

            var tokenHeader = await _headerBearerToken.HeaderTokenRequest();
            if(string.IsNullOrEmpty(tokenHeader))
                return RedirectToAction("UnauthorizedAccess", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenHeader);
            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // إعادة توجيه إلى صفحة Unauthorized
                return RedirectToAction("UnauthorizedAccess", "Account");
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<ProductViewModel>>(content);
                var products = pagedResponse.Items.ToList();
                var TotalItems = pagedResponse.TotalItems;
                ViewData["CurrentPage"] = pageNumber;
                ViewData["PageSize"] = pageSize;
                ViewData["TotalItems"] = TotalItems;
                return View(products);
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> CreateProduct()
        {
            ViewData["Title"] = "Create Product";
            var Categories = await _categoryService.GetCategoriesAsync();
            // تحويل التصنيفات إلى قائمة SelectListItem
            ViewBag.CategoryList = Categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            }).ToList(); 

            var Stores = await _storeService.GetStoresAsync();
            ViewBag.StoreList = Stores.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.StoreName
            }).ToList();
            return View(new Product()); 
        }
      

        [HttpPost]
        public async Task<JsonResult> CreateProduct(Product product, IFormFile ImagePath)
        {
            using (var client = new HttpClient())
            {
                // تعيين الـ URL للـ API الخاص بك

                // إنشاء MultipartFormDataContent لإرسال البيانات مع الملفات
                using (var form = new MultipartFormDataContent())
                {
                    // إضافة بيانات المنتج
                    form.Add(new StringContent(product.Name), "Product.Name");
                    form.Add(new StringContent(product.Code), "Product.Code");
                    form.Add(new StringContent(product.Description??""), "Product.Description");
                    form.Add(new StringContent(product.Price.ToString()), "Product.Price");
                    form.Add(new StringContent(product.StockQuantity?.ToString()), "Product.StockQuantity");
                    form.Add(new StringContent(product.CategoryId.ToString()), "Product.CategoryId");

                    // إضافة الصورة إلى الفورم إذا كانت موجودة
                    if (ImagePath != null && ImagePath.Length > 0)
                    {
                        var streamContent = new StreamContent(ImagePath.OpenReadStream());
                        form.Add(streamContent, "ImagePath", ImagePath.FileName);
                    }

                    // إرسال الطلب إلى الـ API
                    var response = await client.PostAsync(API_EndPoint.CreateProductENDPoint, form);

                    // التحقق من النتيجة
                    if (response.IsSuccessStatusCode)
                    {
                        // الطلب تم بنجاح
                        return Json(new { Message= "Product added successfully!", Success="true" });
                    }
                    else
                    {
                        // خطأ في الطلب
                        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                        return Json(new { Message= "Failed to add the product.", Success="false" });
                    }
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var tokenHeader = await _headerBearerToken.HeaderTokenRequest();
            if (string.IsNullOrEmpty(tokenHeader))
                return RedirectToAction("UnauthorizedAccess", "Account");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenHeader);
            string url = API_EndPoint.EditProductEndPoint(id);
            var response = await _httpClient.GetAsync(url);


            if (response.IsSuccessStatusCode)
            {
                // التعامل مع استجابة ناجحة
                var responseContent = await response.Content.ReadAsStringAsync();
                Product product = JsonConvert.DeserializeObject<Product>(responseContent);
                var Categories = await _categoryService.GetCategoriesAsync();
                var Stocks = await _storeService.GetStoresAsync();

                ViewBag.CategoryList = new SelectList(Categories, "CategoryId", "Name");
                ViewBag.StocksList = new SelectList(Stocks, "StockId", "Name");
                // معالجة المحتوى إذا لزم الأمر
                return View(product);
            }
            else
            {
                return View(new Product());
                // التعامل مع الاستجابة الفاشلة
            }
        }
        
        [HttpPost]
        public async Task<JsonResult> EditProduct([FromForm] ProductViewModel productViewModel)
        {
            var url = API_EndPoint.EditProductAPIENDPoint_Post(productViewModel.ProductId);
            // إعداد البيانات التي سيتم إرسالها عبر POST
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(productViewModel.ProductId.ToString()), "ProductId");
            form.Add(new StringContent(productViewModel.Name), "Name");
            form.Add(new StringContent(productViewModel.Code), "Code");
            form.Add(new StringContent(productViewModel.Price.ToString()), "Price");
            form.Add(new StringContent(productViewModel.StockQuantity.ToString()), "StockQuantity");
            form.Add(new StringContent(productViewModel.Description), "Description");
            form.Add(new StringContent(productViewModel.ImagePath), "ImagePath");
            form.Add(new StringContent(productViewModel.StockId.ToString()), "StockId");
            form.Add(new StringContent(productViewModel.CategoryId.ToString()), "CategoryId");

            var tokenHeader = await _headerBearerToken.HeaderTokenRequest();
            if (string.IsNullOrEmpty(tokenHeader))
                return Json(new { message = "UnauthorizedAccess", success = "false" });

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenHeader);

            var response = await _httpClient.PutAsync(url, form);

            if (response.IsSuccessStatusCode && (int)response.StatusCode == StatusCodes.Status204NoContent)
            {
                return Json(new { message = "Product Updated Successfully", success = "true" });
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return Json(new { message = errorMessage, success = "false" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProductImage([FromForm] ProductImage productImage)
        {
            if (productImage.ImagePath == null || productImage.ImagePath.Length == 0)
                return Json(new { message = "No file uploaded.", success = "false" });

            if (string.IsNullOrEmpty(productImage.Id))
                return Json(new { message = "refresh page and try again.", success = "false" });

            int productId;
            if (!int.TryParse(productImage.Id, out productId))
                return Json(new { message = "Invalid ProductId.", success = "false" });


            try
            {

                productImage.ProductId = productId;
                string url = API_EndPoint.ChangeProductImageEndpoint(productImage);
                // إعداد البيانات التي سيتم إرسالها عبر POST
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(productImage.ProductId.ToString()), "ProductId");
                form.Add(new StringContent(productImage.Id), "Id");

                // إضافة الصورة إلى المحتوى
                if (productImage.ImagePath != null)
                {
                    var imageContent = new StreamContent(productImage.ImagePath.OpenReadStream());
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/webp");
                    form.Add(imageContent, "ImagePath", productImage.ImagePath.FileName);
                }

                // إرسال طلب POST إلى الـ API
                var response = await _httpClient.PostAsync(url, form);



                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    return Json(new {imagePath= content, message = "Image updated successfully.", success = "true" });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return Json(new { message = errorMessage, success = "false" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = "false" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> GetProduct([FromForm] ProductDetails product)
        {
            try
            {
                product.Code = string.IsNullOrEmpty(product.Code) ?"":product.Code;


                var tokenHeader = await _headerBearerToken.HeaderTokenRequest();
                if (string.IsNullOrEmpty(tokenHeader))
                    return RedirectToAction("UnauthorizedAccess", "Account");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenHeader);

                string url = API_EndPoint.GetProductAPIENDPoint(product);
                // إعداد البيانات التي سيتم إرسالها عبر POST
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(product.ProductId.ToString()), "ProductId");
                form.Add(new StringContent(product.Code), "Code");
                form.Add(new StringContent(product.SearchType.ToString()), "SearchType");

               

                // إرسال طلب POST إلى الـ API
                var response = await _httpClient.PostAsync(url, form);



                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Product productViewModel = JsonConvert.DeserializeObject<Product>(responseContent);
                    return Json(new { productViewModel, success = "true" });
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return Json(new { message = errorMessage, success = "false" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, success = "false" });
            }
        }


    }
    public class ResponseAPI
    {
        public string message { get; set; }
    }
    public class ProductImage
    {
        public int ProductId { get; set; }
        public string Id { get; set; } 
        public IFormFile ImagePath { get; set; }
    }

    public class ProductDetails {
        public int ProductId { get; set; }
        public string Code { get; set; } = "";
        public int SearchType { get; set; }
    }
}
