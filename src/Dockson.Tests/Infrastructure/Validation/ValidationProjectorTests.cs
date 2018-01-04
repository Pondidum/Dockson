using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Transformers.Commits;
using Dockson.Infrastructure.Validation;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Infrastructure.Validation
{
	public class ValidationProjectorTests
	{
		private readonly ValidationProjector _validation;

		public ValidationProjectorTests()
		{
			var inner = new InnerProjector();
			_validation = new ValidationProjector(inner);
		}

		[Fact]
		public void When_there_are_no_validators_for_a_message_type()
		{
			Should.NotThrow(() => _validation.Project(new TestNotification { Name = "test", Version = "1" }));
		}

		[Fact]
		public void When_base_properties_are_invalid()
		{
			var ex = Should.Throw<NotificationValidationException>(() => _validation.Project(new TestNotification()));

			ex.Messages.ShouldBe(new[]
			{
				"You must specify the project's name",
				"You must specify the project's version"
			});
		}

		[Fact]
		public void When_base_and_child_validators_have_messages()
		{
			var ex = Should.Throw<NotificationValidationException>(() => _validation.Project(new CommitNotification()));

			ex.Messages.ShouldBe(new[]
			{
				"You must specify the project's name",
				"You must specify the project's version",
				"You must specify the project's branch",
				"You must specify the project's Commit Hash"
			});
		}

		private class TestNotification : Notification
		{
		}

		private class InnerProjector : IProjector
		{
			private readonly List<Notification> _notifications;

			public InnerProjector()
			{
				_notifications = new List<Notification>();
			}

			public IEnumerable<Notification> Notifications => _notifications;
			public void Project(Notification notification) => _notifications.Add(notification);
		}
	}
}
