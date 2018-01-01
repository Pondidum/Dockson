using System;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class DeploymentLeadTimeProjectionTests
	{
		private const string Team = "team-one";

		private readonly View _view;

		private readonly EventSource _serviceOne;
		private readonly EventSource _serviceTwo;

		public DeploymentLeadTimeProjectionTests()
		{
			var updater = new ViewStore();
			var projection = new DeploymentLeadTimeProjection(updater.UpdateDeploymentLeadTime);

			_view = updater.View;
			_serviceOne = EventSource.For(projection, s =>
			{
				s.Name = "service-one";
				s.Groups.Add(Team);
			});
			_serviceTwo = EventSource.For(projection, s =>
			{
				s.Name = "service-two";
				s.Groups.Add(Team);
			});
		}

		[Fact]
		public void When_projecting_one_build()
		{
			_serviceOne.BuildSucceeded();

			_view.ShouldBeEmpty();
		}

		[Fact]
		public void When_projecting_one_deployment()
		{
			_serviceOne.ProductionDeployment();

			_view.ShouldBeEmpty();
		}

		[Fact]
		public void When_projecting_a_build_and_matching_deployment()
		{
			_serviceOne
				.BuildSucceeded()
				.Advance(TimeSpan.FromMinutes(20))
				.ProductionDeployment();

			var summary = _view[_serviceOne.Name].DeploymentLeadTime[_serviceOne.CurrentDay];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(20),
				() => summary.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_builds_and_matching_deployments()
		{
			_serviceOne
				.BuildSucceeded()
				.Advance(TimeSpan.FromMinutes(20))
				.ProductionDeployment()
				.Advance(TimeSpan.FromMinutes(20))
				.NewCommitHash()
				.BuildSucceeded()
				.Advance(TimeSpan.FromMinutes(15))
				.ProductionDeployment();

			var summary = _view[_serviceOne.Name].DeploymentLeadTime[_serviceOne.CurrentDay];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(17.5),
				() => summary.Deviation.ShouldBe(3.53, tolerance: 0.01)
			);
		}

		[Fact]
		public void When_projecting_two_builds_and_matching_deployments_for_different_services()
		{
			_serviceOne
				.BuildSucceeded()
				.Advance(TimeSpan.FromMinutes(20))
				.ProductionDeployment();

			_serviceTwo
				.Advance(TimeSpan.FromMinutes(40))
				.BuildSucceeded()
				.Advance(TimeSpan.FromMinutes(15))
				.ProductionDeployment();

			var one = _view[_serviceOne.Name].DeploymentLeadTime[_serviceOne.CurrentDay];
			var two = _view[_serviceTwo.Name].DeploymentLeadTime[_serviceTwo.CurrentDay];
			var team = _view[Team].DeploymentLeadTime[_serviceOne.CurrentDay];

			_view.ShouldSatisfyAllConditions(
				() => one.Median.ShouldBe(20),
				() => one.Deviation.ShouldBe(0),
				() => two.Median.ShouldBe(15),
				() => two.Deviation.ShouldBe(0),
				() => team.Median.ShouldBe(17.5),
				() => team.Deviation.ShouldBe(3.53, tolerance: 0.01)
			);
		}
	}
}
