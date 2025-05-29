using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Entities;
using MyApp.Models;
using MyApp.Services;
using System.Security.Claims;


namespace MyApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
    {
		private readonly AuthService _authService;

		public AuthController(AuthService authService)
		{
			_authService = authService;
		}
		[HttpPost("register")]
		//public async Task<ActionResult<User>> Register(User request)
		public async Task<ActionResult<string>> Register(UserRegisterDto request)
		{
			var response = await _authService.RegisterAsync(request);
			if (!response.Success)
				return BadRequest("User already exists.");

			return Ok(response);
		}

		[HttpPost("login")]
		//public async Task<ActionResult<TokenResponseDto>> Login(User request)
		public async Task<ActionResult<string>> Login(UserLoginDto request)

		{
			var response = await _authService.LoginAsync(request);
			if (!response.Success)
				return BadRequest("Invalid username or password.");

			return Ok(response);
		}

		[Authorize]
		[HttpGet("profile")]
		public IActionResult GetProfile()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (userId == null)
			{
				return Unauthorized();
			}

			// Now you can use userId to fetch user info, messages, etc.
			return Ok(new { UserId = userId });
		}

	}
}
