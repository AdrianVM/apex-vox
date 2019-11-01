using Microsoft.AspNetCore.Mvc;
using ApexVoxApi.Services;
using System.Collections.Generic;
using ApexVoxApi.Models;

namespace ApexVoxApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : Controller
    {
        private readonly ITenantsService _tenantService;
 
        public TenantsController(ITenantsService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost("create")]
        public IActionResult CreateSharding()
        {
            _tenantService.CreateSharding();

            return Ok();
        }

        [HttpPost("register")]
        public IActionResult RegisterNewTenant([FromBody] string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
            {
                return BadRequest("TenantName cannot be null or emtpy");
            }

            _tenantService.RegisterNewTenant(tenantName);

            return Ok(tenantName);
        }

        [HttpPost("register-db")]
        public IActionResult RegisterCreateNewDb([FromBody] string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
            {
                return BadRequest("TenantName cannot be null or emtpy");
            }

            _tenantService.RegisterNewTenantCreateDb(tenantName);

            return Ok(tenantName);
        }
    }
}