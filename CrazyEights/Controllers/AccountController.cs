using CrazyEights.Data.Migrataions;
using CrazyEights.Dtos.Account;
using CrazyEights.Models;
using CrazyEights.Services;
using CrazyEights.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CrazyEights.Controllers
{
    public class AccountController : MainApiController
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(JWTService jwtService, 
            SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("refresh-application-user")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshApplicationUser()
        {
            var user = await _userManager.FindByNameAsync(User.GetUserName());
            return CreateApplicationUserDto(user);
        }

        [HttpGet("login")]
        public async Task<ActionResult<ApplicationUserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return Unauthorized("Invalid username or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid username or password");

            return CreateApplicationUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (await CheckEmailExistsAsync(model.Email))
            {
                return BadRequest(string.Format("An existing account is using {0}, email address. Please try with another email address", model.Email));
            }

            if (await CheckUsernameExistsAsync(model.PlayerName))
            {
                return BadRequest(string.Format("Player name of {0} is taken. Please try with another name", model.PlayerName));
            }

            var userToAdd = new ApplicationUser
            {
                UserName = model.PlayerName.ToLower(),
                PlayerName = model.PlayerName,
                Email = model.Email.ToLower()
            };

            var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            result = await _userManager.AddToRoleAsync(userToAdd, SD.Role_Player);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("Account has been created!");
        }

        #region Private Helper Methods
        private ApplicationUserDto CreateApplicationUserDto(ApplicationUser user)
        {
            return new ApplicationUserDto
            {
                PlayerName = user.PlayerName,
                JWT = _jwtService.CreateJWT(user)
            };
        }
        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        private async Task<bool> CheckUsernameExistsAsync(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
        #endregion
    }
}
