using MyApp.Models;
using TalksyApp.Helper;
using TalksyApp.Models;
using Dapper;
using MyApp.Entities;

namespace TalksyApp.Services
{
	public class ChatService
	{
		private readonly IConfiguration _configuration;
		private readonly SqlConnectionFactory _sqlConnectionFactory;

		public ChatService(IConfiguration configuration, SqlConnectionFactory sqlConnectionFactory)
		{
			_configuration = configuration;
			_sqlConnectionFactory = sqlConnectionFactory;
		}

		public async Task<ServiceResponse<List<Message>>> GetChats(string sender, string receiver, int offset, int limit)
		{
			ServiceResponse<List<Message>> response = new ServiceResponse<List<Message>>();

			using var connection = _sqlConnectionFactory.Create();
			const string sql = @"
				SELECT *
				FROM Messages
				WHERE 
					(SenderId = @UserA AND ReceiverId = @UserB)
					OR (SenderId = @UserB AND ReceiverId = @UserA)
				ORDER BY SentAt
				OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY
			";

			var messages = (await connection.QueryAsync<Message>(
				sql,
				new { UserA = sender, UserB = receiver , Offset = offset, Limit = limit}
			)).ToList();

			if (!messages.Any())
			{
				return null;
			}

			const string markAsReadSql = @"
				UPDATE Messages
				SET HasRead = 1
				WHERE SenderId = @UserB AND ReceiverId = @UserA AND HasRead = 0;
			";
			await connection.ExecuteAsync(markAsReadSql, new { UserA = sender, UserB = receiver });

			response.Data = messages;
			response.Success = true;
			response.Message = "Messages fetched successfully.";

			return response;
		}

		public async Task<ServiceResponse<List<UserList>>> GetAllUsers(string user)
		{
			var response = new ServiceResponse<List<UserList>>();

			using var connection = _sqlConnectionFactory.Create();

			const string sql = @"
				SELECT Id, Username, COUNT(*) OVER() AS TotalUsers
				FROM Users WHERE Id != @UserId
			";

			var users = (await connection.QueryAsync<UserList>(sql, new { UserId = Guid.Parse(user) })).ToList();

			if (!users.Any())
			{
				response.Success = false;
				response.Message = "No users found.";
				return response;
			}

			response.Data = users;
			response.Success = true;
			response.Message = "Users fetched successfully.";

			return response;
		}


		public async Task<ServiceResponse<string>> SendMessage(MessageDto message)
		{
			var response = new ServiceResponse<string>();
			using var connection = _sqlConnectionFactory.Create();

			const string sql = @"
				INSERT INTO Messages (SenderId, ReceiverId, Content, SentAt, HasRead)
				VALUES (@SenderId, @ReceiverId, @Content, @SentAt, 0);
			";
			 
			var result = await connection.ExecuteAsync(sql, new
			{
				SenderId = message.SenderId,
				ReceiverId = message.ReceiverId,
				Content = message.Content,
				SentAt = DateTime.UtcNow
			});

			if (result > 0)
			{
				response.Success = true;
				response.Message = "Message sent successfully.";
			}
			else
			{
				response.Success = false;
				response.Message = "Failed to send message.";
			}

			return response;
		}



	}
}
