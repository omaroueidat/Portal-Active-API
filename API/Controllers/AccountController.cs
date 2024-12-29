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
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public AccountController(UserManager<AppUser> userManager, TokenService tokenService, IConfiguration config)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri("https://graph.facebook.com")
            };
        }

        // Login Endpoint
        [HttpPost("login")]
        [AllowAnonymous] // Allow unauthoried/unknown users to access the endpoint
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Email == loginDto.Email);

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
                // Return Model state error and validation problem that is more fancier and more usable for the frontend
                ModelState.AddModelError("email", "Email Taken");
                return ValidationProblem();
            }

            // Cheking for duplicate emails again for sending a clear response
            if (await _userManager.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                // Return Model state error and validation problem that is more fancier and more usable for the frontend
                ModelState.AddModelError("username", "Username Taken!");
                return ValidationProblem();
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
            var user = await _userManager.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));

            return CreateUserObject(user);
        }

        // Facebook Login
        [AllowAnonymous]
        [HttpPost("fbLogin")]
        public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
        {
            // Get the keys
            var fbVerifyKeys = _config["Facebook:AppId"] + '|' + _config["Facebook:AppSecret"];

            // Send the request to facebook
            var verifyTokenResponse = await _httpClient.GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");

            // Check if the user is true and that the token is issued from out application
            if (!verifyTokenResponse.IsSuccessStatusCode) return Unauthorized();

            // Get the facebook user info from the facebook api
            // Prepare the Url
            var fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";

            // Send the Request
            var fbInfo = await _httpClient.GetFromJsonAsync<FacebookDto>(fbUrl);

            // Check if the User exist in our application (logged in before)
            var user = await _userManager.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.Email == fbInfo.Email);

            if (user is not null) return CreateUserObject(user);

            // If first time logging In then create a new user instance for the user
            user = new AppUser
            {
                DisplayName = fbInfo.Name,
                Email = fbInfo.Email,
                UserName = fbInfo.Email,
                Photos = new List<Photo>
                {
                    new Photo
                    {
                        Id = "fb_" + fbInfo.Id,
                        Url = fbInfo.Picture.Data.Url,
                        IsMain = true,
                    }
                }
            };

            // Create the user instance
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded) return BadRequest("Problem Creating User Account!");

            return CreateUserObject(user);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = user.Photos?.FirstOrDefault(p => p.IsMain)?.Url,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName,
            };
        }
    }
}
