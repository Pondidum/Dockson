using System.Linq;
using Dockson.Domain.Transformers.Build;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Transformers.Build
{
	public class BuildNotificationValidatorTests
	{
		private string[] Validate(BuildNotification notification) => new BuildNotificationValidator()
			.Validate(notification)
			.ToArray();

		[Fact]
		public void When_the_status_is_missing()
		{
			Validate(new BuildNotification { Version = "1.0.0" }).ShouldBe(new[]
			{
				"You must specify the status of the build (success or failure)"
			});
		}

		[Fact]
		public void When_the_status_is_present()
		{
			Validate(new BuildNotification { Version = "1.0.0", Status = "success" }).ShouldBeEmpty();
		}

		[Fact]
		public void When_the_version_is_missing()
		{
			Validate(new BuildNotification { Status = "success" }).ShouldBe(new[]
			{
				"You must specify the project's version"
			});
		}

		[Fact]
		public void When_the_version_is_present()
		{
			Validate(new BuildNotification { Version = "1.0.0", Status = "success" }).ShouldBeEmpty();
		}
	}
}
