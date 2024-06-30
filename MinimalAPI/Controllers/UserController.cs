using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Data;
using MinimalAPI.Helpers;
using MinimalAPI.Models;

namespace MinimalAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IConfiguration _config;

        public UserController(UserDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet("/Status")]
        public string  Get()
        {
            return "Weater status not ready now";
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Authenticate")]
        public async Task<IActionResult> AddUser(User user)
        {
            if (user == null)
                return BadRequest("User object is null");

            JwtTokenGenerator generator = new JwtTokenGenerator(_config);
            var token = generator.GenerateToken(user);

            if (generator.ValidateToken(token))
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok(new { Token = token });
            }

            return BadRequest("Failed to generate or validate token");
        }
    }
}
