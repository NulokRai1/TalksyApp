using Microsoft.AspNetCore.Mvc;
using MyApp.Entities;
using MyApp.Models;
using MyApp.Services;


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
			var user = await _authService.RegisterAsync(request);
			if (user is null)
				return BadRequest("Username already exists.");

			return Ok("user");
		}

		[HttpPost("login")]
		//public async Task<ActionResult<TokenResponseDto>> Login(User request)
		public async Task<ActionResult<string>> Login(UserLoginDto request)

		{
			var result = await _authService.LoginAsync(request);
			if (result is null)
				return BadRequest("Invalid username or password.");

			return Ok("success");
		}
	}
}
