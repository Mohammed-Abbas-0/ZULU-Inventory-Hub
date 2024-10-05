using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InventoryHubUI.Models
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName +" "+ LastName;
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
    }
    public class CreateCustomerViewModel:CustomerViewModel
    {
        public IFormFile CustomerImage { get; set; }

    }
    public class GetCustomerViewModel: CustomerViewModel
    {
        public string CustomerImageUrl { get; set; }
    } 
    public class EditCustomerViewModel: CustomerViewModel
    {
        public IFormFile CustomerImage { get; set; }
        public string CustomerImageUrl { get; set; }
    }
}
