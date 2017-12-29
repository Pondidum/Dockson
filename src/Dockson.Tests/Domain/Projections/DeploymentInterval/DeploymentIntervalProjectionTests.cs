using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Projections.DeploymentInterval;
using Dockson.Domain.Views;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.DeploymentInterval
{
	public class DeploymentIntervalProjectionTests
	{
		private readonly IntervalView _view;
		private readonly EventSource _service;

		public DeploymentIntervalProjectionTests()
		{
			_view = new IntervalView();
			var projection = new DeploymentIntervalProjection((group, day, newSummary) =>
			{
				_view.TryAdd(group, new GroupSummary<IntervalSummary>());
				_view[group].Daily[day] = newSummary;
			});

			_service = new EventSource(projection);
		}

		[Fact]
		public void When_projecting_one_deployment()
		{
			_service.ProductionDeployment();

			var day = _view[_service.Name].Daily[new DayDate(_service.Timestamp)];

			day.ShouldSatisfyAllConditions(
				() => day.Median.ShouldBe(0),
				() => day.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_deployments()
		{
			_service
				.ProductionDeployment()
				.Advance(1.Hour())
				.ProductionDeployment();

			var day = _view[_service.Name].Daily[new DayDate(_service.Timestamp)];

			day.ShouldSatisfyAllConditions(
				() => day.Median.ShouldBe(1.Hour().TotalMinutes),
				() => day.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_several_deployments_on_the_same_day()
		{
			_service
				.ProductionDeployment()
				.Advance(1.Hour()).ProductionDeployment()
				.Advance(1.Hour()).ProductionDeployment()
				.Advance(2.Hours()).ProductionDeployment()
				.Advance(1.Hour()).ProductionDeployment();

			var day = _view[_service.Name].Daily[new DayDate(_service.Timestamp)];

			_view.ShouldSatisfyAllConditions(
				() => day.Median.ShouldBe(60), //1 hour
				() => day.Deviation.ShouldBe(30) // half hour
			);
		}

		[Fact]
		public void When_projecting_several_deployments_on_several_days()
		{
			var firstDay = _service.Timestamp;
			var secondDay = firstDay.AddDays(1);

			_service
				.ProductionDeployment()
				.Advance(1.Hour()).ProductionDeployment()
				.Advance(1.Hour()).ProductionDeployment()
				.Advance(2.Hours()).ProductionDeployment()
				.Advance(1.Hour()).ProductionDeployment()
				.AdvanceTo(secondDay)
				.Advance(2.Hours()).ProductionDeployment()
				.Advance(2.Hours()).ProductionDeployment()
				.Advance(1.Hour()).ProductionDeployment();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].Daily[new DayDate(firstDay)].Median.ShouldBe(60), //1 hour
				() => _view[_service.Name].Daily[new DayDate(firstDay)].Deviation.ShouldBe(30), // half hour
				() => _view[_service.Name].Daily[new DayDate(secondDay)].Median.ShouldBe(120), //2 hours
				() => _view[_service.Name].Daily[new DayDate(secondDay)].Deviation.ShouldBe(676.165, tolerance: 0.005)
			);
		}
	}
}
