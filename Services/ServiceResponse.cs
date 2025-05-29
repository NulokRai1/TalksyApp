namespace TalksyApp.Services
{
	public class ServiceResponse<T>
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }
		public List<string> Errors { get; set; }

		public ServiceResponse()
		{
			Errors = new List<string>();
		}

		public static ServiceResponse<T> SuccessResponse(T data, string message = null)
		{
			return new ServiceResponse<T>
			{
				Success = true,
				Message = message ?? "Operation completed successfully",
				Data = data
			};
		}

		public static ServiceResponse<T> ErrorResponse(string message, List<string> errors = null)
		{
			return new ServiceResponse<T>
			{
				Success = false,
				Message = message,
				Errors = errors ?? new List<string>()
			};
		}
	}
}
