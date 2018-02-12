using System;
using Dockson.Domain;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain
{
	public class PreProjectorTests
	{
		private readonly PreProjector _projector;
		private readonly DateTime _timestamp;

		public PreProjectorTests()
		{
			_timestamp = DateTime.UtcNow;
			_projector = new PreProjector(new InnerProjector(), () => _timestamp);
		}

		[Fact]
		public void When_the_notification_timestamp_is_not_set()
		{
			var notification = new Notification();
			_projector.Project(notification);

			notification.Timestamp.ShouldBe(_timestamp);
		}

		[Fact]
		public void When_the_notification_timestamp_is_set()
		{
			var existing = DateTime.UtcNow.AddDays(-5);
			var notification = new Notification { Timestamp = existing };
			_projector.Project(notification);

			notification.Timestamp.ShouldBe(existing);
		}

		[Fact]
		public void When_a_name_contains_periods()
		{
			var notification = new Notification { Name = "what.is.this" };
			_projector.Project(notification);

			notification.Name.ShouldBe("what-is-this");
		}

		[Fact]
		public void When_a_group_contains_periods()
		{
			var notification = new Notification { Groups = { "one.with.periods", "one-without" } };
			_projector.Project(notification);

			notification.Groups.ShouldBe(new[] { "one-with-periods", "one-without" });
		}

		private class InnerProjector : IProjector
		{
			public void Project(Notification notification)
			{
			}
		}
	}
}
