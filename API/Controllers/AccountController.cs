using API.DTO;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        // Login Endpoint
        [HttpPost("login")]
        [AllowAnonymous] // Allow unauthoried/unknown users to access the endpoint
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null)
            {
                return Unauthorized();
            }

            // Check the password if its valid or not (create a hash and comapre it with the database)
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (result)
            {
                return CreateUserObject(user);
            }
            return Unauthorized();
        }

        // Register Endpoint
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // Check for duplicate emails -> Done inside the Identity Core Setup
            // Check for duplicate Usernames
            if (await _userManager.Users.AnyAsync(u => u.UserName == registerDto.Username))
            {
                return BadRequest("Username is already taken!");
            }

            // Cheking for duplicate emails again for sending a clear response
            if (await _userManager.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return BadRequest("Email is already taken!");
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username,
            };

            // Add the User to the database
            var result = await _userManager.CreateAsync(user, registerDto.Password);
                
            // Check if the result is succeeded, else there might be a problem with the password
            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }

            return BadRequest(result.Errors);
        }


        // Get the current user from the token
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            return CreateUserObject(user);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = null,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName,
            };
        }
    }
}
