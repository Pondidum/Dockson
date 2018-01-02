using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Storage;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class BuildIntervalProjectionTests
	{
		private readonly Dictionary<string, GroupView> _view;
		private readonly EventSource _service;

		public BuildIntervalProjectionTests()
		{
			var updater = new DictionaryViewStore();
			var projection = new BuildIntervalProjection(updater.UpdateBuildInterval);

			_view = updater.View;
			_service = EventSource.For(projection);
		}

		[Fact]
		public void When_projecting_one_build()
		{
			_service.BuildSucceeded();

			var day = _view[_service.Name].BuildInterval[_service.CurrentDay];

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

			var day = _view[_service.Name].BuildInterval[_service.CurrentDay];

			day.ShouldSatisfyAllConditions(
				() => day.Median.ShouldBe(1.Hour().TotalMinutes),
				() => day.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_several_builds_on_the_same_day()
		{
			_service
				.BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded()
				.Advance(2.Hours()).BuildSucceeded()
				.Advance(1.Hour()).BuildSucceeded();

			var day = _view[_service.Name].BuildInterval[_service.CurrentDay];

			_view.ShouldSatisfyAllConditions(
				() => day.Median.ShouldBe(60), //1 hour
				() => day.Deviation.ShouldBe(30) // half hour
			);
		}

		[Fact]
		public void When_projecting_several_builds_on_several_days()
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
				() => _view[_service.Name].BuildInterval[new DayDate(firstDay)].Median.ShouldBe(60), //1 hour
				() => _view[_service.Name].BuildInterval[new DayDate(firstDay)].Deviation.ShouldBe(30), // half hour
				() => _view[_service.Name].BuildInterval[new DayDate(secondDay)].Median.ShouldBe(120), //2 hours
				() => _view[_service.Name].BuildInterval[new DayDate(secondDay)].Deviation.ShouldBe(676.165, tolerance: 0.005)
			);
		}
	}
}
