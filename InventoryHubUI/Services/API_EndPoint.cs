using InventoryHubUI.Controllers;
using InventoryHubUI.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHubUI.Services
{
    public static class API_EndPoint
    {
        public static string APIURI = "https://localhost:44324/";
        public static string AccountEndPoint = APIURI + "api/Account/";
        public static string LoGinENDPoint = APIURI + "api/Account/login";
        public static string RegisterENDPoint = APIURI + "api/Account/register";
        public static string LogoutENDPoint = APIURI + "api/Account/logout";
        public static string RefreshTokenENDPoint = APIURI + "api/Account/refreshtoken";

        // Product END  Point
        public static string ProductENDPoint = APIURI + "api/Products/";
        public static string CreateProductENDPoint = ProductENDPoint + "createproduct";
        public static string EditProductEndPoint(int id) => $"{ProductENDPoint}{id}";
        public static string ChangeProductImageEndpoint(ProductImage productImage)
        {
            // يمكن استخدام JsonConvert لتسلسل الكائن إلى JSON
            var json = JsonConvert.SerializeObject(productImage);

            // قم بترميز الـ JSON
            var encodedData = Uri.EscapeDataString(json);

            return $"{ProductENDPoint}ChangeProductImage?data={encodedData}";
        }

        public static string GetProductAPIENDPoint(ProductDetails Product)
        {
            // يمكن استخدام JsonConvert لتسلسل الكائن إلى JSON
            var json = JsonConvert.SerializeObject(Product);

            // قم بترميز الـ JSON
            var encodedData = Uri.EscapeDataString(json);

            return $"{ProductENDPoint}GetProduct?data={encodedData}";
        }

        public static string EditProductAPIENDPoint_Post(int id)
        {
            return $"{ProductENDPoint}{id}";
        }
        // Categories End Point
        public static string Category_ENDPoint = APIURI + "api/Category/";
        public static string CategoryENDPoint = APIURI + "api/Category/getcategories";


        // Stores  END  Point
        public static string StoreENDPoint = APIURI + "api/Store/getstores";

        #region Customers
        public static string CustomerENDPoint = APIURI + "api/Customer/";
        public static string GetCustomerENDPoint = CustomerENDPoint + "GetCustomers";
        public static string GetCustomerByIdENDPoint = CustomerENDPoint + "GetCustomerById";
        public static string ChangeStatusENDPoint = CustomerENDPoint + "ChangeStatus";
        public static string DeleteCustomerENDPoint = CustomerENDPoint + "DeleteCustomer";
        public static string CreateCustomerENDPoint = CustomerENDPoint + "CreateCustomer";
        public static string EditCustomerENDPoint = CustomerENDPoint + "EditCustomer";


        #endregion

    }
}
