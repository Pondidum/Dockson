using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain;
using Dockson.Domain.Transformers.Deployment;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Transformers.Deployment
{
	public class DeploymentTransformerTests
	{
		private readonly DeploymentTransformer _transformer;
		private readonly List<object> _events;

		public DeploymentTransformerTests()
		{
			_events = new List<object>();
			_transformer = new DeploymentTransformer();
		}

		[Fact]
		public void When_the_notification_is_successful_non_production_deployment()
		{
			var notification = Deployment("success", environment: "test");

			_transformer.Transform(notification, _events.Add);

			_events.ShouldBeEmpty();
		}

		[Fact]
		public void When_the_notification_is_failed_non_production_deployment()
		{
			var notification = Deployment("failure", environment: "test");

			_transformer.Transform(notification, _events.Add);

			_events.ShouldBeEmpty();
		}

		[Fact]
		public void When_the_notification_is_successful_production_deployment()
		{
			var notification = Deployment("success");

			_transformer.Transform(notification, _events.Add);

			_events.ShouldHaveSingleItem().ShouldBeOfType<ProductionDeployment>();
		}

		[Fact]
		public void When_the_notification_is_failed_production_deployment()
		{
			var notification = Deployment("failure");

			_transformer.Transform(notification, _events.Add);

			_events.ShouldBeEmpty();
		}

		private DeploymentNotification Deployment(string status, string environment = "production") => new DeploymentNotification
		{
			Timestamp = DateTime.UtcNow,
			Source = "github",
			Name = "SomeService",
			Version = "1.0.0",
			Status = status,
			Environment = environment
		};
	}
}
