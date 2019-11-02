using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApexVoxApi.Models;
using ApexVoxApi.Services;
using ApexVoxApi.ViewModels;

namespace ApexVoxApi.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] Credentials credentials)
        {
            var user = _userService.Authenticate(credentials.UserName, credentials.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpPost("create-for-tenant")]
        public IActionResult Create([FromBody] string tenantName)
        {
            var user = _userService.Create(tenantName);

            return Ok(user);
        }
    }
}
