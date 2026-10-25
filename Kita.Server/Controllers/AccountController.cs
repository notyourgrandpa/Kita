using Kita.Application.DTOs;
using Kita.Infrastructure.Identity;
using Kita.Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Kita.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController: ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtHandler _jwtHandler;
        public AccountController(UserManager<ApplicationUser> userManager, JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(ApiLoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                return Unauthorized(new ApiLoginResult()
                {
                    Success = false,
                    Message = "Invalid Email or Password."
                });
            }
            var secToken = await _jwtHandler.GetTokenAsync(user);
            var jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
            var userDto = new UserDTO
            {
                Id = user.Id,
                Name = user.UserName!,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "",
                Email = user.Email,
                AvatarUrl = null
            };
            return Ok(new ApiLoginResult()
            {
                Success = true,
                Message = "Login successful",
                Token = jwt,
                User = userDto
            });
        }
    }
}