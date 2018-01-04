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
			Validate(new DeploymentNotification { Status = "wat" }).ShouldBe(new[]
			{
				"You must specify the project's deployment environment (Production, QA, Test etc)"
			});
		}

		[Fact]
		public void When_the_status_is_missing()
		{
			Validate(new DeploymentNotification { Environment = "hash" }).ShouldBe(new[]
			{
				"You must specify the project's deployment status (success, failure etc)"
			});
		}

		[Fact]
		public void When_both_environment_and_status_are_missing()
		{
			Validate(new DeploymentNotification()).ShouldBe(new[]
			{
				"You must specify the project's deployment environment (Production, QA, Test etc)",
				"You must specify the project's deployment status (success, failure etc)"
			});
		}

		[Fact]
		public void When_both_environment_and_status_are_present()
		{
			Validate(new DeploymentNotification { Status = "wat", Environment = "hashhhh"}).ShouldBeEmpty();
		}
	}
}
