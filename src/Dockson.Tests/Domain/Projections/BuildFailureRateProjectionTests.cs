using Dockson.Domain;
using Dockson.Domain.Projections;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class BuildFailureRateProjectionTests
	{
		private readonly View _view;
		private readonly EventSource _service;

		public BuildFailureRateProjectionTests()
		{
			_view = new View();
			var projection = new BuildFailureRateProjection(_view.UpdateBuildFailureRate);

			_service = EventSource.For(projection, s => s.Name = "wat-service");
		}

		[Fact]
		public void When_a_build_is_successful()
		{
			_service.BuildSucceeded();

			_view[_service.Name]
				.BuildFailureRate[_service.CurrentDay]
				.Rate
				.ShouldBe(0);
		}

		[Fact]
		public void When_a_build_is_unsuccessful()
		{
			_service.BuildFailed();

			_view[_service.Name]
				.BuildFailureRate[_service.CurrentDay]
				.Rate
				.ShouldBe(100);
		}

		[Fact]
		public void When_multiple_builds_are_a_mix()
		{
			_service
				.BuildFailed()
				.BuildSucceeded()
				.BuildSucceeded()
				.BuildFailed();

			_view[_service.Name]
				.BuildFailureRate[_service.CurrentDay]
				.Rate
				.ShouldBe(50);
		}

		[Fact]
		public void When_multiple_builds_are_a_mix_over_several_days()
		{
			var firstDay = _service.Timestamp;
			var secondDay = firstDay.AddDays(1);

			_service
				.BuildFailed()
				.BuildSucceeded()
				.BuildSucceeded()
				.AdvanceTo(secondDay)
				.BuildFailed()
				.BuildSucceeded()
				.BuildFailed();

			var group = _view[_service.Name];

			group.ShouldSatisfyAllConditions(
				() => group.BuildFailureRate[new DayDate(firstDay)].Rate.ShouldBe(33.33, tolerance: 0.01),
				() => group.BuildFailureRate[new DayDate(secondDay)].Rate.ShouldBe(66.66, tolerance: 0.01)
			);
		}
	}
}
