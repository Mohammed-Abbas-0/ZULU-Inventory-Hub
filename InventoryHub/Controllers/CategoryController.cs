using AutoMapper;
using InventoryHub.Helper.FilebaseStorage;
using InventoryHub.Helper.RedisCaching;
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
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryQueryService _categoryQueryService;
        private readonly IMapper _mapper;
        private readonly IRedisDistributedCache<List<CategoryViewModel>> _distributedCache;

        public CategoryController(IUnitOfWork unitOfWork, ICategoryQueryService categoryQueryService,IMapper mapper, IRedisDistributedCache<List<CategoryViewModel>> distributedCache)
        {
            _unitOfWork = unitOfWork;
            _categoryQueryService = categoryQueryService;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet("getcategories")]
        public async Task<ActionResult<Category>> GetCategoriesAsync()
        {
          

                var Categories = await _unitOfWork.Categories.GetAllAsync();
                return Ok(Categories);
            

        }

        #region CREATE  Category
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryViewModel categoryViewModel)
        {
            if (categoryViewModel is null || string.IsNullOrWhiteSpace(categoryViewModel.Code) || string.IsNullOrWhiteSpace(categoryViewModel.Name))
                return BadRequest();
            // Ignore Spaces  and Swap
            string Code = categoryViewModel.Code.Trim();
            string Name = categoryViewModel.Name.Trim();
            categoryViewModel.Code = Code;
            categoryViewModel.Name = Name;

            var category = _mapper.Map<Category>(categoryViewModel);

            await _unitOfWork.Categories.AddAsync(category);

            // After Updated
            var categoryCreated = await _categoryQueryService.GetCategoryByCodeAsync(categoryViewModel.Code);
            if (categoryCreated is null)
                return BadRequest();

            var categoryViewModelCreated = _mapper.Map<CategoryViewModel>(categoryCreated);
            return Ok(categoryViewModelCreated);
        }

        #endregion

        #region  Get Categories
        [HttpGet]
        public async Task<ActionResult> GetCategoriesWithCountProducts(int pageNumber = 1, int pageSize = 25)
        {
            //string key = $"Categories_{pageNumber}_{pageSize}";
            string key = $"Categories_{pageNumber}_{pageSize}";
            var data = await _distributedCache.GetEntryAsync(key);
            if (data is not null)
            {
                var categories = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return Ok(categories);
            }
            else
            {



                if (pageNumber < 1)
                {
                    return BadRequest("Page number must be greater than 0.");
                }
                if (pageSize < 1 || pageSize > 100) // يمكنك تعديل الحد الأقصى حسب الحاجة
                {
                    return BadRequest("Page size must be between 1 and 100.");
                }

                try
                {
                    var categories = await _categoryQueryService.GetCategoriesAsync(pageNumber, pageSize);
                    await _distributedCache.SetEntryAsync(key, categories);
                    return Ok(categories);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }
        }
        #endregion

        #region  Get Count Of Categories
        [HttpGet("GetCountCategories")]
        public async Task<ActionResult> GetCountCategories()
        {
            var getCountCategories = await _categoryQueryService.GetCountCategoriesAsync();
            return Ok(getCountCategories);
            
        }
        #endregion

        #region Get Category By Id
        [HttpGet("getcategorybyid")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category is null)
                return BadRequest();
            var categoryViewModel = _mapper.Map<CategoryViewModel>(category);
            return Ok(categoryViewModel);
        }

        #endregion

        #region Update Category
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromForm] CategoryViewModel categoryViewModel)
        {
            if (categoryViewModel is null)
                return BadRequest();
            int categoryId = categoryViewModel.CategoryId;
            var category = _mapper.Map<Category>(categoryViewModel);

            await _unitOfWork.Categories.UpdateAsync(category);

            // After Updated
            var categoryUpdated = await _unitOfWork.Categories.GetByIdAsync(categoryId);
            var categoryViewModelUpdated = _mapper.Map<CategoryViewModel>(categoryUpdated);
            return Ok(categoryViewModelUpdated);
        }
        #endregion


        #region Delete Category
        [HttpDelete]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id == 0)
                return BadRequest();
            bool isDeleted = await _unitOfWork.Categories.DeleteAsync(id);
            if(!isDeleted)
                return BadRequest();
            return NoContent();
        }
        #endregion
    }
}
