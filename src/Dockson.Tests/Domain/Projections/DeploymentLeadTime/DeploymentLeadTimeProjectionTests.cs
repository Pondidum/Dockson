using System;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Projections.DeploymentLeadTime;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.DeploymentLeadTime
{
	public class DeploymentLeadTimeProjectionTests
	{
		private const string Team = "team-one";

		private readonly LeadTimeView _view;

		private readonly EventSource _serviceOne;
		private readonly EventSource _serviceTwo;

		public DeploymentLeadTimeProjectionTests()
		{
			_view = new LeadTimeView();
			var projection = new DeploymentLeadTimeProjection(_view);

			_serviceOne = new EventSource(projection) { Name = "service-one", Groups = { Team } };
			_serviceTwo = new EventSource(projection) { Name = "service-two", Groups = { Team } };
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

			var summary = _view[_serviceOne.Name].Daily[new DayDate(_serviceOne.Timestamp)];

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

			var summary = _view[_serviceOne.Name].Daily[new DayDate(_serviceOne.Timestamp)];

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

			var one = _view[_serviceOne.Name].Daily[new DayDate(_serviceOne.Timestamp)];
			var two = _view[_serviceTwo.Name].Daily[new DayDate(_serviceTwo.Timestamp)];
			var team = _view[Team].Daily[new DayDate(_serviceOne.Timestamp)];

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
