using Dockson.Domain;
using Dockson.Domain.Projections.BuildFailureRate;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.BuildFailureRate
{
	public class BuildFailureRateProjectionTests
	{
		private readonly BuildFailureRateView _view;
		private readonly EventSource _service;

		public BuildFailureRateProjectionTests()
		{
			_view = new BuildFailureRateView();
			var projection = new BuildFailureRateProjection(_view);

			_service = new EventSource(projection)
			{
				Name = "wat-service"
			};
		}

		[Fact]
		public void When_a_build_is_successful()
		{
			_service.BuildSucceeded();

			_view[_service.Name]
				.Daily[new DayDate(_service.Timestamp)]
				.FailureRate
				.ShouldBe(0);
		}

		[Fact]
		public void When_a_build_is_unsuccessful()
		{
			_service.BuildFailed();

			_view[_service.Name]
				.Daily[new DayDate(_service.Timestamp)]
				.FailureRate
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
				.Daily[new DayDate(_service.Timestamp)]
				.FailureRate
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
				() => group.Daily[new DayDate(firstDay)].FailureRate.ShouldBe(33.33, tolerance: 0.01),
				() => group.Daily[new DayDate(secondDay)].FailureRate.ShouldBe(66.66, tolerance: 0.01)
			);
		}
	}
}
