using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dockson.Infrastructure.Validation
{
	public class NotificationValidationFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			var exception = context.Exception as NotificationValidationException;

			if (exception == null)
				return;

			var response = new ValidationResponse
			{
				Exception = "Validation Exception",
				Messages = exception.Messages
			};

			context.ExceptionHandled = true;
			context.Result = new JsonResult(response)
			{
				StatusCode = (int)HttpStatusCode.BadRequest
			};
		}
	}

	public class ValidationResponse
	{
		public string Exception { get; set; }
		public IEnumerable<string> Messages { get; set; }
	}
}
