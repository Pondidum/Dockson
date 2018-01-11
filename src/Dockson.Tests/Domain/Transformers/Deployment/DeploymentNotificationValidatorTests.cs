using System.Linq;
using Dockson.Domain.Transformers.Deployment;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Transformers.Deployment
{
	public class DeploymentNotificationValidatorTests
	{
		private string[] Validate(DeploymentNotification notification) => new DeploymentNotificationValidator()
			.Validate(notification)
			.ToArray();

		[Fact]
		public void When_the_environment_hash_is_missing()
		{
			Validate(new DeploymentNotification { Version = "2.0.0", Status = "wat" }).ShouldBe(new[]
			{
				"You must specify the project's deployment environment (Production, QA, Test etc)"
			});
		}

		[Fact]
		public void When_the_status_is_missing()
		{
			Validate(new DeploymentNotification { Version = "2.0.0", Environment = "hash" }).ShouldBe(new[]
			{
				"You must specify the project's deployment status (success, failure etc)"
			});
		}

		[Fact]
		public void When_the_version_is_missing()
		{
			Validate(new DeploymentNotification { Status = "wat", Environment = "hash" }).ShouldBe(new[]
			{
				"You must specify the project's version"
			});
		}

		[Fact]
		public void When_all_are_present()
		{
			Validate(new DeploymentNotification { Version = "2.0.0", Status = "wat", Environment = "hashhhh"}).ShouldBeEmpty();
		}
	}
}
