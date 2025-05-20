using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Entities;
using MyApp.Models;

namespace MyApp.Services
{
	public class AuthService(AppDbContext context, IConfiguration configuration)
	{
		//public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request)
		public async Task<string?> LoginAsync(UserLoginDto request)

		{
			var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
			if (user is null)
			{
				return null;
			}
			if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
				== PasswordVerificationResult.Failed)
			{
				return null;
			}

			//return await CreateTokenResponse(user);
			return ("success");
		}
		public async Task<User?> RegisterAsync(UserRegisterDto request)
		{
			if (await context.Users.AnyAsync(u => u.Username == request.Username))
			{
				return null;
			}

			var user = new User();
			var hashedPassword = new PasswordHasher<User>()
				.HashPassword(user, request.Password);

			user.Username = request.Username;
			user.PasswordHash = hashedPassword;

			context.Users.Add(user);
			await context.SaveChangesAsync();

			return user;
		}

	}
}
