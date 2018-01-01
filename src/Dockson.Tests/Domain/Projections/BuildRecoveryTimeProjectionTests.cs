using Dockson.Domain;
using Dockson.Domain.Projections;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class BuildRecoveryTimeProjectionTests
	{
		private readonly View _view;
		private readonly EventSource _service;
		private readonly EventSource _serviceTwo;

		public BuildRecoveryTimeProjectionTests()
		{
			var updater = new ViewStore();
			var projection = new BuildRecoveryTimeProjection(updater.UpdateBuildRecoveryTime);

			_view = updater.View;
			_service = EventSource.For(projection, s => s.Name = "ServiceOne");
			_serviceTwo = EventSource.For(projection, s => s.Name = "ServiceTwo");
		}

		[Fact]
		public void When_a_build_recovers()
		{
			_service
				.BuildFailed()
				.Advance(20.Minutes())
				.BuildSucceeded();

			var daySummary = _view[_service.Name].BuildRecoveryTime[_service.CurrentDay];

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

			var daySummary = _view[_service.Name].BuildRecoveryTime[_service.CurrentDay];

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

			var serviceOne = _view[_service.Name].BuildRecoveryTime[_service.CurrentDay];

			serviceOne.ShouldSatisfyAllConditions(
				() => serviceOne.Median.ShouldBe(20),
				() => serviceOne.Deviation.ShouldBe(0)
			);

			var serviceTwo = _view[_serviceTwo.Name].BuildRecoveryTime[_serviceTwo.CurrentDay];

			serviceTwo.ShouldSatisfyAllConditions(
				() => serviceTwo.Median.ShouldBe(15),
				() => serviceTwo.Deviation.ShouldBe(7.071, tolerance: 0.001)
			);
		}
	}
}
