using Azure;
using Azure.Core;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyApp.Data;
using MyApp.Entities;
using MyApp.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TalksyApp.Helper;
using TalksyApp.Models;
using TalksyApp.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MyApp.Services
{
	public class AuthService(AppDbContext context, IConfiguration configuration, SqlConnectionFactory sqlConnectionFactory)
	{
		//public async Task<TokenResponseDto?> LoginAsync(UserLoginDto request)
		public async Task<ServiceResponse<string>> LoginAsync(UserLoginDto request)
		{
			var response = new ServiceResponse<string>();

			using var connection = sqlConnectionFactory.Create();

			const string sql = "SELECT Id, Username, Email, PasswordHash FROM Users WHERE Email = @Email";
			var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = request.Email });

			if (user == null)
			{
				response.Success = false;
				response.Message = "User not found.";
				return response;
			}

			var passwordCheck = new PasswordHasher<User>()
				.VerifyHashedPassword(user, user.PasswordHash, request.Password);

			if (passwordCheck == PasswordVerificationResult.Failed)
			{
				response.Success = false;
				response.Message = "Invalid password.";
				return response;
			}
			var token = await CreateTokenResponse(user);

			const string sql2 =
			"""
				MERGE Tokens AS target
				USING(SELECT @userId AS UserId, @at AS AccessToken, DATEADD(DAY, 1, GETDATE()) AS AccessTokenExpiry) AS source
				ON target.UserId = source.UserId
				WHEN MATCHED THEN
				UPDATE SET

					target.AccessToken = source.AccessToken,
			        target.AccessTokenExpiry = source.AccessTokenExpiry
				WHEN NOT MATCHED THEN

					INSERT(UserId, AccessToken, AccessTokenExpiry)

					VALUES(source.UserId, source.AccessToken, source.AccessTokenExpiry);


			MERGE Users AS target
			USING (
			    SELECT @userId AS Id, @refreshToken AS RefreshToken, DATEADD(DAY, 7, GETDATE()) AS RefreshTokenExpiryTime
			) AS source
			ON target.Id = source.Id
			WHEN MATCHED THEN
			    UPDATE SET
			        target.RefreshToken = source.RefreshToken,
			        target.RefreshTokenExpiryTime = source.RefreshTokenExpiryTime
			WHEN NOT MATCHED THEN
			    INSERT (Id, RefreshToken, RefreshTokenExpiryTime)
			    VALUES (source.Id, source.RefreshToken, source.RefreshTokenExpiryTime);
			""";
			await connection.ExecuteAsync(sql2, new { userId = user.Id, at = token.AccessToken, refreshToken = token.RefreshToken });
			response.Data = token.AccessToken;
			response.Success = true;
			return response;
		}

		public async Task<ServiceResponse<string>> RegisterAsync(UserRegisterDto request)
		{
			var response = new ServiceResponse<string>();
			using var connection = sqlConnectionFactory.Create();

			const string sql = "SELECT Email FROM Users WHERE Email = @Email";
			var result = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = request.Email });

			//if (await context.Users.AnyAsync(u => u.Username == request.Username))
			if (result != null)
			{
				return null;
			}

			var user = new User();
			var hashedPassword = new PasswordHasher<User>()
				.HashPassword(user, request.Password);

			const string sql2 = @"
				INSERT INTO Users (Username, PasswordHash, Role, Email)
				VALUES (@Username, @PasswordHash, @Role, @Email)";

			await connection.ExecuteAsync(sql, new
			{
				Username = request.Username,
				PasswordHash = hashedPassword,
				Role = "User",
				Email = request.Email
			});

			response.Data = "success";
			response.Success = true;
			return response;
		}

		private async Task<TokenResponseDto> CreateTokenResponse(User? user)
		{
			return new TokenResponseDto
			{
				AccessToken = CreateToken(user),
				RefreshToken = GenerateRefreshToken()
			};
		}
	

		private string CreateToken(User user)
		{
			var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.Username),
					new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
					new Claim(ClaimTypes.Role, user.Role)
				};

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

			var tokenDescriptor = new JwtSecurityToken(
				issuer: configuration.GetValue<string>("AppSettings:Issuer"),
				audience: configuration.GetValue<string>("AppSettings:Audience"),
				claims: claims,
				expires: DateTime.UtcNow.AddDays(1),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
		}

		private string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}
		public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
		{
			var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
			if (user is null)
				return null;

			return await CreateTokenResponse(user);
		}

		private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
		{
			var user = await context.Users.FindAsync(userId);
			if (user is null || user.RefreshToken != refreshToken
				|| user.RefreshTokenExpiryTime <= DateTime.UtcNow)
			{
				return null;
			}

			return user;
		}

	}
}
