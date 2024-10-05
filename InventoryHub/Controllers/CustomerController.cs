using AutoMapper;
using InventoryHub.Helper.FilebaseStorage;
using InventoryHub.Models;
using InventoryHub.ModelViews;
using InventoryHub.ServicesPatterns.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerQueryService _customerQuery;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;

        public CustomerController(IUnitOfWork unitOfWork, ICustomerQueryService customerQuery, IMapper mapper, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _customerQuery = customerQuery;
            _mapper = mapper;
            _storageService = storageService;
        }

        #region GET  CUSTOMERS
        [HttpGet("GetCustomers")]
        public async Task<IActionResult> GetCustomers(int pageNumber = 1, int pageSize = 25)
        {
            var customers = await _customerQuery.GeCutomersAsync(pageNumber,pageSize);
            if (customers is null || customers.Count() == 0)
                return BadRequest("Not found any Customers Created.");
            try
            {
                var customerViewModel = _mapper.Map<List<CustomerViewModel>>(customers);
                return Ok(customerViewModel);
            } 
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }

        }

        #endregion

        #region GET CUSTOMER by ID
        [HttpGet("GetCustomerById/{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Customer Id");
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer is null)
                return BadRequest("Customer Not Found");
            try
            { 
                var customerViewModel = _mapper.Map<CustomerViewModel>(customer);
                customerViewModel.CustomerImageUrl = customer.CustomerImage;
                return Ok(customerViewModel);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

        #region  CREATE  CUSTOMER
        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer([FromForm] CustomerViewModel customerViewModel)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if (customerViewModel is null)
                return BadRequest("Try to Created Customer Again.");

            var imagePath = customerViewModel.CustomerImage;
            if (imagePath == null || imagePath.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
           

            try
            {


                if(customerViewModel.CustomerImage != null)
                    customerViewModel.CustomerImageUrl = customerViewModel.CustomerImage.FileName;
                var customer = _mapper.Map<Customer>(customerViewModel);

                bool isExisted = await _customerQuery.IsExisted(customer);
                if (isExisted)
                    return BadRequest("Customer Name or Email already exists.");


                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.CompleteAsync();

                Tuple<string, bool> firebaseStorage = await _storageService.Upload(customer.CustomerId,imagePath, "customer");

                if (firebaseStorage.Item2)
                {
                    // تحديث المنتج بمسار الصورة بعد رفعها
                    customer.CustomerImage = firebaseStorage.Item1;

                    // تحديث المنتج في قاعدة البيانات
                    await _unitOfWork.Customers.UpdateAsync(customer);
                    await _unitOfWork.CompleteAsync();

                    return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
                }
                else
                {
                    return BadRequest(firebaseStorage.Item2);
                }


            }
            catch (Exception ex)
            {
                return BadRequest($"Try to Created Customer Again, { ex.Message}");
            }

        }
        #endregion

        #region  EDIT  CUSTOMER
        [HttpPatch("EditCustomer")]
        public async Task<IActionResult> EditCustomer([FromForm] CustomerViewModel customerViewModel)
        {
            if (customerViewModel is null)
                return BadRequest("Try to Edit Customer Again.");

            var imagePath = customerViewModel.CustomerImage;
           

            try
            {
                if (customerViewModel.CustomerImage != null)
                    customerViewModel.CustomerImageUrl = customerViewModel.CustomerImage.FileName;
                var customer = _mapper.Map<Customer>(customerViewModel);
                await _unitOfWork.Customers.UpdateAsync(customer);
                await _unitOfWork.CompleteAsync();

                if (imagePath != null && imagePath.Length > 0)
                {
                    // هحذف القديم واضيف الجديد
                    var imageIsDeleted = await _storageService.DeleteAsync(customer.CustomerImage);
                    if (imageIsDeleted)
                    {
                        Tuple<string, bool> firebaseStorage = await _storageService.Upload(customer.CustomerId, imagePath, "customer");

                        if (firebaseStorage.Item2)
                        {
                            customer.CustomerImage = firebaseStorage.Item1;

                            await _unitOfWork.Customers.UpdateAsync(customer);
                            await _unitOfWork.CompleteAsync();

                            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
                        }
                        else
                        {
                            return BadRequest(firebaseStorage.Item2);
                        }
                    }
                }
                else
                {
                    return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Try to Created Customer Again, { ex.Message}");
            }

            return BadRequest("Try to Edit Customer Again.");
        }
        #endregion


        #region CHANGE STATUS
        [HttpPost("ChangeStatus")]
        public async Task<IActionResult> ChangeStatus([FromQuery] int CustomerId)
        {
            if (CustomerId <= 0)
                return BadRequest("Invalid Customer Id");
            var statusChanged = await _customerQuery.ChangeStatus(CustomerId);

            if(statusChanged is false)
                return BadRequest("Customer Not Found");

            try
            {
                var customerStatus = await _unitOfWork.Customers.GetByIdAsync(CustomerId);
                return Ok(customerStatus.IsActive);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region  Delete Customer
        [HttpDelete("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer([FromQuery] int CustomerId)
        {
            if (CustomerId <= 0)
                return BadRequest("Invalid Customer Id");
            var customerDeleted = await _unitOfWork.Customers.DeleteAsync(CustomerId);

            if (customerDeleted is false)
                return BadRequest("Customer Not Found");

           return NoContent();
            
        }
        #endregion
    }
}
