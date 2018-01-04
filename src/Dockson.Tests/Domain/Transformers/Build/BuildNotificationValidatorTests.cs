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
			Validate(new BuildNotification { }).ShouldBe(new[]
			{
				"You must specify the status of the build (success or failure)"
			});
		}

		[Fact]
		public void When_the_status_is_present()
		{
			Validate(new BuildNotification { Status = "success" }).ShouldBeEmpty();
		}
	}
}
