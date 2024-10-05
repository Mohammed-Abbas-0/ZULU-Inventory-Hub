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
using InventoryHubUI.ModelViews;

namespace InventoryHubUI.Controllers
{

    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var url = $"{API_EndPoint.Category_ENDPoint}?pageNumber={pageNumber}&pageSize={pageSize}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<Category>>(content);

                string apiGetCountCategories = $"{API_EndPoint.Category_ENDPoint}GetCountCategories";
                var responseGetCountCategories = await _httpClient.GetAsync(apiGetCountCategories);
                var contentGetCountCategories = await responseGetCountCategories.Content.ReadAsStringAsync();

                ViewData["CurrentPage"] = pageNumber;
                ViewData["PageSize"] = pageSize;
                ViewData["TotalItems"] = Convert.ToInt32(contentGetCountCategories);
                return View(categories);
            }
            return View();
        }

        public async Task<IActionResult> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync(API_EndPoint.CategoryENDPoint);

            // التحقق من النتيجة
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var Categories = JsonConvert.DeserializeObject<List<Category>>(content);

                // الطلب تم بنجاح
                return Json(Categories);
            }
            else
            {
                return Json(new List<Category>());
            }
        }


        public async Task<IActionResult> GetCategory(int id)
        {
            string endPoint = $"{API_EndPoint.Category_ENDPoint}getcategorybyid?id={id}";
            var response = await _httpClient.GetAsync(endPoint);

            // التحقق من النتيجة
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<Category>(content);

                // الطلب تم بنجاح
                return Json(new { category });
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromForm] Category category)
        {
            var url = $"{API_EndPoint.Category_ENDPoint}CreateCategory";
            // إعداد البيانات التي سيتم إرسالها عبر POST
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(category.Name), "Name");
            form.Add(new StringContent(category.Code), "Code");

            var response = await _httpClient.PostAsync(url, form);

            if (response.IsSuccessStatusCode && (int)response.StatusCode == StatusCodes.Status200OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categoryCreated = JsonConvert.DeserializeObject<Category>(content);
                return Json(new { message = "Create Category Successfully", success = "true", categoryCreated });
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return Json(new { message = errorMessage, success = "false" });
            }

        }

        [HttpPost]
        public async Task<IActionResult> EditCategory([FromForm] Category category)
        {
                var url = $"{API_EndPoint.Category_ENDPoint}";
                // إعداد البيانات التي سيتم إرسالها عبر POST
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(category.CategoryId.ToString()), "CategoryId");
                form.Add(new StringContent(category.Name), "Name");
                form.Add(new StringContent(category.Code), "Code");

                var response = await _httpClient.PutAsync(url, form);
                
                if (response.IsSuccessStatusCode && (int)response.StatusCode == StatusCodes.Status200OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categoryUpdated = JsonConvert.DeserializeObject<Category>(content);
                    return Json(new { message = "Category Updated Successfully", success = "true" ,categoryUpdated});
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return Json(new { message = errorMessage, success = "false" });
                }
            
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _httpClient.DeleteAsync($"{API_EndPoint.APIURI}api/Category?id={id}");
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode && (int)response.StatusCode == StatusCodes.Status204NoContent)
            {
                return Json(new { message="Categor Deleted Succesfully.",success="true"});
            }
            return Json(new { message="please try again.",success="false"});
        }
    }

}
