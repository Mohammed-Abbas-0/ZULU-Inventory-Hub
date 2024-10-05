using InventoryHub.Models;
using InventoryHub.ServicesPatterns.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Storage;
using InventoryHub.Helper.FilebaseStorage;
using Microsoft.EntityFrameworkCore;
using InventoryHub.ModelViews;
using AutoMapper;
using InventoryHub.Helper.HelperMethods;
using InventoryHub.ServicesPatterns.Implementation;
using Microsoft.AspNetCore.Authorization;

namespace InventoryHub.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IMapper _mapper;
        private readonly IProductQueryService _productQueryService;
        private readonly IHelperMethod _helperMethod;
        private readonly ISearchStrategyFactory _searchStrategyFactory;
       
        public ProductsController(IUnitOfWork unitOfWork, IStorageService storageService, IMapper mapper, IProductQueryService productQueryService
                            , IHelperMethod helperMethod, ISearchStrategyFactory searchStrategyFactory)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _mapper = mapper;
            _productQueryService = productQueryService;
            _helperMethod= helperMethod;
            _searchStrategyFactory= searchStrategyFactory;
        }

        #region  Get Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductViewModel>>> GetProducts(int pageNumber=1,int pageSize=25)
        {
            // تحقق من أن pageNumber و pageSize قيم صحيحة
            if (pageNumber < 1)
            {
                return BadRequest("Page number must be greater than 0.");
            }
            if (pageSize < 1 || pageSize > 100) // يمكنك تعديل الحد الأقصى حسب الحاجة
            {
                return BadRequest("Page size must be between 1 and 100.");
            }
            var products = await _productQueryService.GetProductsWithCategoryAndStockAsync(pageNumber,pageSize);
            // تحقق من وجود بيانات
            if (products == null || !products.Any())
            {
                return NotFound("No products found.");
            }
            int ProductsCount = await _productQueryService.GetTotalProductsCountAsync();
            var productViewModel = _mapper.Map<IEnumerable<ProductViewModel>>(products);

            var response = new PagedResponse<ProductViewModel>
            {
                Items = productViewModel,
                TotalItems = ProductsCount
            };

            return Ok(response);
        }
        #endregion

        #region Get BY  ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        #endregion

        #region CREATE  Product
        [HttpPost("createproduct")]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] CreateProductRequest ProductRequest)
        {
            var product = ProductRequest.Product;
            

            var imagePath = ProductRequest.ImagePath;
            if (imagePath == null || imagePath.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            // إدخال المنتج كمؤقت في قاعدة البيانات دون صورة
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            // بعد الإدخال، قم برفع الصورة باستخدام الـ ProductId المخصص
            Tuple<string, bool> firebaseStorage = await _storageService.Upload(product.ProductId, imagePath,"product");

            if (firebaseStorage.Item2)
            {
                // تحديث المنتج بمسار الصورة بعد رفعها
                product.ImagePath = firebaseStorage.Item1;

                // تحديث المنتج في قاعدة البيانات
                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
            }
            else
            {
                return BadRequest(firebaseStorage.Item2);
            }
        }

        public class CreateProductRequest
        {
            public Product Product { get; set; }
            public IFormFile ImagePath { get; set; }
        }
        #endregion

        #region Update  Product

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id,[FromForm] ProductViewModel productViewModel)
        {
            if (id != productViewModel.ProductId)
            {
                return BadRequest();
            }

            var product = _mapper.Map<Product>(productViewModel);
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
        #endregion

        #region Delete  Product

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _unitOfWork.Products.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return NoContent();
        }
        #endregion

        #region Change Image


        [HttpPost("ChangeProductImage")]
        public async Task<IActionResult> ChangeProductImage([FromForm] ProductImage productImage)
        {
            if (productImage.ImagePath == null || productImage.ImagePath.Length == 0)
            {
                return BadRequest("Invalid image file.");
            }

            // رفع الصورة إلى Firebase Storage
            Tuple<string, bool> firebaseStorage = await _storageService.Upload(productImage.ProductId,productImage.ImagePath,"product");
            if (!firebaseStorage.Item2)
            {
                return BadRequest("Failed to upload image to storage.");
            }
            var product = await _unitOfWork.Products.GetByIdAsync(productImage.ProductId);
            if(!string.IsNullOrEmpty(product.ImagePath))
            {
                // must be remove old image from Firebase
                await _storageService.DeleteAsync(product.ImagePath);
            }

            // تحديث مسار الصورة في قاعدة البيانات
            bool isImageChanged = await _productQueryService.ChangeProductImage(productImage.ProductId, firebaseStorage.Item1);
            if (isImageChanged)
            {
                return Ok(firebaseStorage.Item1);
            }
            else
            {
                // عملية التراجع عن حفظ الصورة في Firebase Storage
                await _storageService.DeleteAsync(firebaseStorage.Item1);
                return BadRequest("Failed to update image in the database.");
            }
        }
        #endregion

        #region Get Product By Query

        [HttpPost("GetProduct")]
        public async Task<IActionResult> GetProduct([FromForm] ProductViewModel product)
        {
            // Use Strategy Pattern
            var searchStrategy = _searchStrategyFactory.GetSearchStrategy(product.SearchType??0);
            Product getProduct = await searchStrategy.Search(product.ProductId,product.Code);
            if (getProduct is null)
                return BadRequest("Product Not Found.");
            
            var productViewModel = _mapper.Map<ProductViewModel>(getProduct);
            return Ok(productViewModel);
        }
        #endregion

        
    }

}
