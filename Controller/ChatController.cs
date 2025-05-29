using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Entities;
using MyApp.Models;
using System.Security.Claims;
using TalksyApp.Helper;
using TalksyApp.Models;
using TalksyApp.Services;

namespace TalksyApp.Controllers 
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChatController : ControllerBase
	{
		private readonly ChatService _chatService;
		private readonly GetIdentity _getIdentity;

		public ChatController(ChatService chatService, GetIdentity getIdentity)
		{
			_chatService = chatService;
			_getIdentity = getIdentity;
		}

		[HttpPost("SendChat")]
		public async Task<IActionResult> SendChat(MessageDto message)
		{
			try
			{
				var response = await _chatService.SendMessage(message);
				return Ok(response);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Getting Message: " + ex.Message);
				return StatusCode(500, "Internal server error: " + ex.Message);
			}

		}

		[HttpGet("Chats")]
		public async Task<IActionResult> UserChat(int page = 1, int pageSize = 10, string receiver = "")
		{
			try
			{
				int offset = (page - 1) * pageSize;
				string sender = _getIdentity.GetUserId(); 
				var response = await _chatService.GetChats(sender, receiver, offset, pageSize);
				return Ok(response);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Getting Message: " + ex.Message);
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}

		[Authorize]
		[HttpGet("GetAllUsers")]
		public async Task<IActionResult> AllUsers()
		{
			try
			{
				var user = _getIdentity.GetUserId();
				var response = await _chatService.GetAllUsers(user);
				return Ok(response);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error in Getting Message: " + ex.Message);
				return StatusCode(500, "Internal server error: " + ex.Message);
			}
		}
	}
}
