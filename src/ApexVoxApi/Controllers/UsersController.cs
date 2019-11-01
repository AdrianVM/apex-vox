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

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            // only allow admins to access other user records
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
            {
                return Forbid();
            }

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
