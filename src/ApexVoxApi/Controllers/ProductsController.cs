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

        // GET: api/Products/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(long productId)
        {
            var product = _unitOfWork.Products.SingleOrDefault(x => x.Id == productId);
            if(product == null)
            {
                return BadRequest();
            }

            return Ok(product);
        }

        // POST: api/Products
        [HttpPost]
        public void Post([FromBody] string value)
        {
            var product = new Product()
            {
                Name = value
            };

            _unitOfWork.Products.Add(product);
            _unitOfWork.SaveChanges();
        }
    }
}
