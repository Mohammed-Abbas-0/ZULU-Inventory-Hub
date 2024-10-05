using InventoryHubUI.Models;
using InventoryHubUI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InventoryHubUI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _httpClient;
        public CustomerController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index()
        {
            var resonse = await _httpClient.GetAsync(API_EndPoint.GetCustomerENDPoint);
            if(resonse.IsSuccessStatusCode)
            {
                string content = await resonse.Content.ReadAsStringAsync();
                var Customers = JsonConvert.DeserializeObject<List<CustomerViewModel>>(content);
                return View(Customers);
            }
            return View(new CustomerViewModel());
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerViewModel customerView)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(customerView.FirstName), "FirstName");
            formData.Add(new StringContent(customerView.LastName), "LastName");
            formData.Add(new StringContent(customerView.Address), "Address");
            formData.Add(new StringContent(customerView.Email), "Email");
            formData.Add(new StringContent(customerView.PhoneNumber), "PhoneNumber");

            if (customerView.CustomerImage != null)
            {
                var fileStream = customerView.CustomerImage.OpenReadStream();
                var fileContent = new StreamContent(fileStream);

                // إضافة رأس الملف لمزيد من المعلومات مثل نوع المحتوى واسم الملف
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(customerView.CustomerImage.ContentType);

                // إضافة الملف إلى الفورم
                formData.Add(fileContent, "CustomerImage", customerView.CustomerImage.FileName);
            }

            string url = $"{API_EndPoint.CreateCustomerENDPoint}";

            var response = await _httpClient.PostAsync(url,formData);

            if(response.IsSuccessStatusCode )
            {
                var Content = await response.Content.ReadAsStringAsync();

                if(response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var jsonConvert = JsonConvert.DeserializeObject<GetCustomerViewModel>(Content);
                    return Json(new { Customer = jsonConvert,success="true" });

                }


            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var msgError = await response.Content.ReadAsStringAsync();
                return Json(new { message = msgError, success = "false" });
            }
            return Json(new {message="Try Again", success="false" });
        }

        

        [HttpGet("GetCustomerById")]
        public async Task<IActionResult> GetCustomerById(int CustomerId)
        {
            if (CustomerId <= 0)
                return Json(new { message="Invalid Customer Id",success="false"});

            string url = $"{API_EndPoint.GetCustomerByIdENDPoint}/{CustomerId}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var Content = await response.Content.ReadAsStringAsync();
                var jsonConvert = JsonConvert.DeserializeObject<GetCustomerViewModel>(Content);
                return Json(new { Customer = jsonConvert, success = "true" });
            }

            return Json(new { message="Invalid Customer Id",success="false"});
        }

        [HttpPost("EditCustomer")]
        public async Task<IActionResult> EditCustomer(EditCustomerViewModel customerViewModel)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(customerViewModel.CustomerId.ToString()), "CustomerId");
            formData.Add(new StringContent(customerViewModel.FirstName), "FirstName");
            formData.Add(new StringContent(customerViewModel.LastName), "LastName");
            formData.Add(new StringContent(customerViewModel.Address), "Address");
            formData.Add(new StringContent(customerViewModel.Email), "Email");
            formData.Add(new StringContent(customerViewModel.PhoneNumber), "PhoneNumber");
            formData.Add(new StringContent(customerViewModel.CustomerImageUrl), "CustomerImageUrl");

            if (customerViewModel.CustomerImage != null)
            {
                var fileStream = customerViewModel.CustomerImage.OpenReadStream();
                var fileContent = new StreamContent(fileStream);

                // إضافة رأس الملف لمزيد من المعلومات مثل نوع المحتوى واسم الملف
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(customerViewModel.CustomerImage.ContentType);

                // إضافة الملف إلى الفورم
                formData.Add(fileContent, "CustomerImage", customerViewModel.CustomerImage.FileName);
            }

            string url = $"{API_EndPoint.EditCustomerENDPoint}";


            var response = await _httpClient.PatchAsync(url,formData);

            if (response.IsSuccessStatusCode)
            {
                var Content = await response.Content.ReadAsStringAsync();
                var jsonConvert = JsonConvert.DeserializeObject<GetCustomerViewModel>(Content);
                return Json(new { Customer = jsonConvert, success = "true" });
            }

            return Json(new { message = "Try Again", success = "false" });
        }

        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(int CustomerId)
        {
            if (CustomerId <= 0)
                return Json(new { message = "Invalid Customer Id", success = "false" });

            string url = $"{API_EndPoint.ChangeStatusENDPoint}/?CustomerId={CustomerId}";

            var response = await _httpClient.PostAsync(url,null);

            if (response.IsSuccessStatusCode)
            {
                var Content = await response.Content.ReadAsStringAsync();
                var isActive = JsonConvert.DeserializeObject<string>(Content);
                return Json(new { isActive, success = "true" });
            }

            return Json(new { message = "Invalid Customer Id", success = "false" });
        }

        [HttpPost("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(int CustomerId)
        {
            if (CustomerId <= 0)
                return Json(new { message = "Invalid Customer Id", success = "false" });

            string url = $"{API_EndPoint.DeleteCustomerENDPoint}/?CustomerId={CustomerId}";

            var response = await _httpClient.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { message = "Customer Deleted Successfuly", success = "true" });
            }

            return Json(new { message = "Invalid Customer Id", success = "false" });
        }
    }


}
