using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IS.Models;
using IS.Models.UserDto;
using IS.Services;
using IS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IS.Controllers
{
    [Area("v1")]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserService _userService;
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userMgr, UserService userService)
        {
            _context = context;
            _userManager = userMgr;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var profiles = ProfileDto.GetUserProfiles(users);
            return Ok(profiles);
        }

        [HttpGet]
        [Route("api/userclaims")]
        [Authorize]
        public IActionResult GetClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute]Guid id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id.ToString()).ConfigureAwait(false);
            if (user == null)
                return NotFound();
            var profile = new ProfileDto(user);
            return Ok(profile);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody]UpdateUserDto updateUser, [FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var userId = User.Claims.FirstOrDefault(p => p.Type==ClaimTypes.NameIdentifier)?.Value;
            if (id.ToString() != userId)
                return BadRequest();
            var isUser = _userManager.Users.Any(u => u.Id == id.ToString());
            if (!isUser)
                return NotFound();
            var updatedUser = await _userService.UpdateUserAsync(id, updateUser).ConfigureAwait(false);
            var saved = await _userService.SaveAsync().ConfigureAwait(false);
            if (!saved)
                return BadRequest();
            var profileUser = new ProfileDto(updatedUser);
            return Ok(profileUser);
        }
    }
}