using CrazyEights.Data.Migrataions;
using CrazyEights.Dtos.Account;
using CrazyEights.Models;
using CrazyEights.Services;
using CrazyEights.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CrazyEights.Controllers
{
    public class AccountController : MainApiController
    {
        private readonly JWTService _jwtService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;

        public AccountController(JWTService jwtService, 
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager,
            IConfiguration config,
            EmailService emailService)
        {
            _jwtService = jwtService;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _emailService = emailService;
        }

        [Authorize]
        [HttpGet("refresh-application-user")]
        public async Task<ActionResult<ApplicationUserDto>> RefreshApplicationUser()
        {
            var user = await _userManager.FindByNameAsync(User.GetUserName());
            return CreateApplicationUserDto(user);
        }

        [HttpPost("login")]
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

            if (await SendConfirmEmail(userToAdd))
            {
                return Ok("Please login to your email and confirm your email addrees.");
            }

            return BadRequest(SD.EmailSentFailureMessage);
        }

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(SD.UnregisteredEmailMessage);

            if (user.EmailConfirmed == true) return BadRequest("Your email was confirmed before");

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return Ok("Your email is confirmed. You can now login and enjoy your stay.");
                }

                return BadRequest(SD.InvalidTokenMessage);
            }
            catch(Exception)
            {
                return BadRequest(SD.InvalidTokenMessage);
            }
        }

        [HttpPost("resend-email-confirmation-link/{email}")]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized(SD.UnregisteredEmailMessage);

            if (user.EmailConfirmed == true) return BadRequest("Your email was confirmed before");

            if (await SendConfirmEmail(user))
            {
                return Ok("Email confirmation link has been resent. Please confirm your email.");
            }

            return BadRequest(SD.EmailSentFailureMessage);
        }

        [HttpPost("forgot-username-or-password/{email}")]
        public async Task<IActionResult> ForgotUsernameOrPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized(SD.UnregisteredEmailMessage);

            if (user.EmailConfirmed == false) return BadRequest(SD.UnconfirmedEmailMessage);

            if (await SendForgotUsernameOrPasswordEmail(user))
            {
                return Ok("Forgot username or password email has been sent. Please check your email");
            }

            return BadRequest(SD.EmailSentFailureMessage);
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized(SD.UnregisteredEmailMessage);

            if (user.EmailConfirmed == false) return BadRequest(SD.UnconfirmedEmailMessage);

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
                if (result.Succeeded)
                {
                    return Ok("Your password has been reset.");
                }

                return BadRequest(SD.InvalidTokenMessage);
            }
            catch (Exception)
            {
                return BadRequest(SD.InvalidTokenMessage);
            }
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

        private async Task<bool> SendConfirmEmail(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_config["Email:ConfirmEmailUrl"]}?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.PlayerName}</p>" +
                "<p>Please confirm your email address by clicking on the following link.</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                "<p>Thank you,</p>" +
                $"<br>{_config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email, "Confirm your email", body);
            return await _emailService.SendEmailAsync(emailSend);
        }

        private async Task<bool> SendForgotUsernameOrPasswordEmail(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_config["Email:ResetPasswordUrl"]}?token={token}&email={user.Email}&username={user.UserName}";

            var body = $"<p>Hello: {user.PlayerName}</p>" +
               $"<p>Username: {user.UserName}</p>" +
               "<p>To reset your password click on the following link.</p>" +
               $"<p><a href=\"{url}\">Click here</a></p>" +
               "<p>Thank you,</p>" +
               $"<br>{_config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email, "Forgot username or password", body);
            return await _emailService.SendEmailAsync(emailSend);
        }
        #endregion
    }
}
