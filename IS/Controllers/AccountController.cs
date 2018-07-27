using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IS.Models;
using IS.Models.AccountDto;
using IS.Models.UserDto;

namespace IS.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController
        (
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new ApplicationUser { UserName = model.UserName, FirstName = model.FirstName, LastName = model.LastName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            string role = "User";

            if (result.Succeeded)
            {
                if (await _roleManager.FindByNameAsync(role) == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);
                await _userManager.AddClaimAsync(user, new Claim("userName", user.UserName));
                await _userManager.AddClaimAsync(user, new Claim("firstName", user.FirstName));
                await _userManager.AddClaimAsync(user, new Claim("lastName", user.LastName));
                await _userManager.AddClaimAsync(user, new Claim("email", user.Email));
                await _userManager.AddClaimAsync(user, new Claim("role", role));

                return Ok(new ProfileDto(user));
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] LoginDto loginModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var login = await _signInManager.PasswordSignInAsync(loginModel.Login, loginModel.Password,
                loginModel.RememberMe, false);

            return Ok(login.Succeeded);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}