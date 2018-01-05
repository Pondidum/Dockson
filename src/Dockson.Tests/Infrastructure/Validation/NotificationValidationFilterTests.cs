using System.Collections.Generic;
using System.Net;
using System.Threading;
using Dockson.Domain;
using Dockson.Infrastructure.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Infrastructure.Validation
{
	public class NotificationValidationFilterTests
	{
		private readonly NotificationValidationFilter _filter;
		private readonly ExceptionContext _context;

		public NotificationValidationFilterTests()
		{
			var actionContext = new ActionContext
			{
				HttpContext = new DefaultHttpContext(),
				RouteData = new RouteData(),
				ActionDescriptor = new ActionDescriptor(),
				ModelState = { }
			};

			_filter = new NotificationValidationFilter();
			_context = new ExceptionContext(actionContext, new List<IFilterMetadata>())
			{
				Result = new OkResult(),
				ExceptionHandled = false
			};
		}

		[Fact]
		public void When_there_is_no_exception()
		{
			_filter.OnException(_context);
			_context.Result.ShouldBeOfType<OkResult>();
		}

		[Fact]
		public void When_the_exception_is_not_a_validation_exception()
		{
			_context.Exception = new AbandonedMutexException();

			_filter.OnException(_context);

			_context.ShouldSatisfyAllConditions(
				() => _context.Exception.ShouldBeOfType<AbandonedMutexException>(),
				() => _context.ExceptionHandled.ShouldBeFalse(),
				() => _context.Result.ShouldBeOfType<OkResult>()
			);
		}

		[Fact]
		public void When_the_exception_is_a_validation_exception()
		{
			_context.Exception = new NotificationValidationException(new Notification(), new string[]
			{
				"one",
				"two",
				"three"
			});

			_filter.OnException(_context);
			var result = _context.Result as JsonResult;
			var response = result?.Value as ValidationResponse;

			_context.ShouldSatisfyAllConditions(
				() => _context.ExceptionHandled.ShouldBeTrue(),
				() => _context.Result.ShouldBeOfType<JsonResult>(),
				() => result?.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest),
				() => response?.Exception.ShouldBe("Validation Exception"),
				() => response?.Messages.ShouldBe(new[] { "one", "two", "three" })
			);
		}
	}
}
