using System;
using System.Collections.Generic;
using System.Linq;
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

		[Fact]
		public void When_a_failed_build_becomes_successful()
		{
			_transformer.Transform(Build("failure"), _events.Add);
			_transformer.Transform(Build("success"), _events.Add);

			_events.Select(e => e.GetType()).ShouldBe(new[]
			{
				typeof(BuildFailed),
				typeof(BuildSucceeded)
			});
		}

		[Fact]
		public void When_a_failed_build_becomes_successful_and_the_version_changes()
		{
			var failure = Build("failure");
			var success = Build("success");

			failure.Version = "1.0.0.123";
			success.Version = "1.0.0.456"; // so many builds to fix it...

			_transformer.Transform(failure, _events.Add);
			_transformer.Transform(success, _events.Add);

			_events.Select(e => e.GetType()).ShouldBe(new[]
			{
				typeof(BuildFailed),
				typeof(BuildSucceeded)
			});
		}

		private BuildNotification Build(string status, DateTime? timestamp = null) => new BuildNotification
		{
			Timestamp = timestamp ?? DateTime.UtcNow,
			Source = "github",
			Name = "SomeService",
			Version = "1.0.0",
			Status = status
		};
	}
}
