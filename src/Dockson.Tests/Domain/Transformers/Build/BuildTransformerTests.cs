using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Transformers.Build;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Transformers.Build
{
	public class BuildTransformerTests
	{
		private readonly BuildTransformer _transformer;
		private readonly List<object> _events;

		public BuildTransformerTests()
		{
			_events = new List<object>();
			_transformer = new BuildTransformer();
		}

		[Fact]
		public void When_the_notification_is_not_a_build()
		{
			var notification = Build("success");
			notification.Type = Stages.Commit;

			_transformer.Transform(notification, _events.Add);

			_events.ShouldBeEmpty();
		}

		[Fact]
		public void When_the_notification_is_a_successful_build()
		{
			var notification = Build("success");

			_transformer.Transform(notification, _events.Add);

			_events
				.ShouldHaveSingleItem()
				.ShouldBeOfType<BuildSucceeded>();
		}

		[Fact]
		public void When_the_notification_is_a_failed_build()
		{
			var notification = Build("failure");

			_transformer.Transform(notification, _events.Add);

			_events
				.ShouldHaveSingleItem()
				.ShouldBeOfType<BuildFailed>();
		}

		[Fact]
		public void When_the_notification_is_anything_other_than_success_build()
		{
			var notification = Build("wfwewg");

			_transformer.Transform(notification, _events.Add);

			_events
				.ShouldHaveSingleItem()
				.ShouldBeOfType<BuildFailed>();
		}

		private Notification Build(string status) => new Notification
		{
			Type = Stages.Build,
			TimeStamp = DateTime.UtcNow,
			Source = "github",
			Name = "SomeService",
			Version = "1.0.0",
			Status = status
		};
	}
}
