using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Projections.BuildInterval;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.BuildInterval
{
	public class BuildIntervalProjectionTests
	{
		private readonly IntervalView _view;
		private readonly EventSource _service;

		public BuildIntervalProjectionTests()
		{
			_view = new IntervalView();
			var projection = new BuildIntervalProjection(_view);

			_service = new EventSource(projection);
		}

		[Fact]
		public void When_projecting_one_build()
		{
			_service.BuildSucceeded();

			var day = _view[_service.Name].Daily[new DayDate(_service.Timestamp)];

			day.ShouldSatisfyAllConditions(
				() => day.Median.ShouldBe(0),
				() => day.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_builds()
		{
			_service
				.BuildSucceeded()
				.Advance(1.Hour())
				.BuildSucceeded();

			var day = _view[_service.Name].Daily[new DayDate(_service.Timestamp)];

			day.ShouldSatisfyAllConditions(
				() => day.Median.ShouldBe(1.Hour().TotalMinutes),
				() => day.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_several_commits_on_the_same_day()
		{
			_service
				.BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded()
				.Advance(2.Hours()).BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded();

			var day = _view[_service.Name].Daily[new DayDate(_service.Timestamp)];

			_view.ShouldSatisfyAllConditions(
				() => day.Median.ShouldBe(60), //1 hour
				() => day.Deviation.ShouldBe(30) // half hour
			);
		}

		[Fact]
		public void When_projecting_several_commits_on_several_days()
		{
			var firstDay = _service.Timestamp;
			var secondDay = firstDay.AddDays(1);

			_service
				.BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded()
				.Advance(2.Hours()).BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded()
				.AdvanceTo(secondDay)
				.Advance(2.Hours()).BuildSucceeded()
				.Advance(2.Hours()).BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].Daily[new DayDate(firstDay)].Median.ShouldBe(60), //1 hour
				() => _view[_service.Name].Daily[new DayDate(firstDay)].Deviation.ShouldBe(30), // half hour
				() => _view[_service.Name].Daily[new DayDate(secondDay)].Median.ShouldBe(120), //2 hours
				() => _view[_service.Name].Daily[new DayDate(secondDay)].Deviation.ShouldBe(676.165, tolerance: 0.005)
			);
		}
	}
}
