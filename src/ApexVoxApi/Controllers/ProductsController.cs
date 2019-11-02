using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApexVoxApi.Infrastructure;
using ApexVoxApi.Models;
using ApexVoxApi.Services;

namespace ApexVoxApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        private readonly ITenantsService _tenantService;

        public ProductsController(IUnitOfWork unitOfWork, IProductService productService, ITenantsService tenantsService )
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
            _tenantService = tenantsService;
        }

        // GET: api/Products
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return _unitOfWork.Products;
        }

        [HttpPost("load-data")]
        public IEnumerable<Product> LoadData(string tenantName)
        {
            var tenant = _tenantService.GetTenantByName(tenantName);

            var products = _productService.LoadData(tenant);

            return products;
        }
    }
}
