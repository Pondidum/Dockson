using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Projections.BuildRecoveryTime;
using Dockson.Domain.Views;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.BuildRecoveryTime
{
	public class BuildRecoveryTimeProjectionTests
	{
		private readonly LeadTimeView _view;
		private readonly EventSource _service;
		private readonly EventSource _serviceTwo;

		public BuildRecoveryTimeProjectionTests()
		{
			_view = new LeadTimeView();
			var projection = new BuildRecoveryTimeProjection((group, day, newSummary) =>
			{
				_view.TryAdd(@group, new GroupSummary<LeadTimeSummary>());
				_view[@group].Daily[day] = newSummary;
			});

			_service = new EventSource(projection) { Name = "ServiceOne" };
			_serviceTwo = new EventSource(projection) { Name = "ServiceTwo" };
		}

		[Fact]
		public void When_a_build_recovers()
		{
			_service
				.BuildFailed()
				.Advance(20.Minutes())
				.BuildSucceeded();

			var daySummary = _view[_service.Name].Daily[new DayDate(_service.Timestamp)];

			daySummary.ShouldSatisfyAllConditions(
				() => daySummary.Median.ShouldBe(20),
				() => daySummary.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_multiple_builds_recovers()
		{
			_service
				.BuildFailed()
				.Advance(20.Minutes())
				.BuildSucceeded()
				.Advance(5.Minutes())
				.BuildFailed()
				.Advance(10.Minutes())
				.BuildSucceeded();

			var daySummary = _view[_service.Name].Daily[new DayDate(_service.Timestamp)];

			daySummary.ShouldSatisfyAllConditions(
				() => daySummary.Median.ShouldBe(15),
				() => daySummary.Deviation.ShouldBe(7.071, tolerance: 0.001)
			);
		}

		[Fact]
		public void When_multiple_service_builds_recovers()
		{
			_service
				.BuildFailed().Advance(20.Minutes()).BuildSucceeded();

			_serviceTwo
				.BuildFailed().Advance(10.Minutes()).BuildSucceeded()
				.Advance(5.Minutes())
				.BuildFailed().Advance(20.Minutes()).BuildSucceeded();
				
			var serviceOne = _view[_service.Name].Daily[new DayDate(_service.Timestamp)];

			serviceOne.ShouldSatisfyAllConditions(
				() => serviceOne.Median.ShouldBe(20),
				() => serviceOne.Deviation.ShouldBe(0)
			);

			var serviceTwo = _view[_serviceTwo.Name].Daily[new DayDate(_serviceTwo.Timestamp)];

			serviceTwo.ShouldSatisfyAllConditions(
				() => serviceTwo.Median.ShouldBe(15),
				() => serviceTwo.Deviation.ShouldBe(7.071, tolerance: 0.001)
			);
		}
	}
}
