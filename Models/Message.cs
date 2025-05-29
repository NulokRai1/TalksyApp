using Microsoft.AspNetCore.Http.HttpResults;

namespace TalksyApp.Models
{
	public class Message
	{
		public Guid Id { get; set; }
		public Guid SenderId { get; set; }
		public Guid ReceiverId { get; set; }
		public string Content { get; set; }
		public bool HasRead { get; set; }
		public DateTime SentAt { get; set; }
	}
}

