using InventoryHub.Helper.FilebaseStorage;
using InventoryHub.Models;
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
    public class StoreController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public StoreController(IUnitOfWork unitOfWork, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        [HttpGet("getstocks")]
        public async Task<ActionResult<Store>> GetStocksAsync()
        {
            var Stocks = await _unitOfWork.Stores.GetAllAsync();
            return Ok(Stocks);
        }
    }
}
