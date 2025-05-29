using System.Security.Claims;

namespace TalksyApp.Helper
{
	public class GetIdentity
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public GetIdentity(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public string GetUserId()
		{
			return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		}

		public string GetUserName()
		{
			return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
		}

		public string GetEmail()
		{
			return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
		}
	}
}
