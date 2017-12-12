using Dockson.Domain;
using Dockson.Domain.Projections.MasterInterval;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.MasterInterval
{
	public class MasterIntervalProjectionTests
	{
		private readonly MasterIntervalView _view;
		private readonly EventSource _service;

		public MasterIntervalProjectionTests()
		{
			_view = new MasterIntervalView();
			var projection = new MasterIntervalProjection(_view);

			_service = new EventSource(projection);
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_service.MasterCommit();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].Daily[new DayDate(_service.Timestamp)].Median.ShouldBe(0),
				() => _view[_service.Name].Daily[new DayDate(_service.Timestamp)].Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_commits_on_the_same_day()
		{
			_service
				.MasterCommit()
				.Advance(1.Hour())
				.MasterCommit();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].Daily[new DayDate(_service.Timestamp)].Median.ShouldBe(60), //1 hour
				() => _view[_service.Name].Daily[new DayDate(_service.Timestamp)].Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_several_commits_on_the_same_day()
		{
			_service
				.MasterCommit()
				.Advance(1.Hour()).MasterCommit()
				.Advance(1.Hour()).MasterCommit()
				.Advance(2.Hours()).MasterCommit()
				.Advance(1.Hour()).MasterCommit();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].Daily[new DayDate(_service.Timestamp)].Median.ShouldBe(60), //1 hour
				() => _view[_service.Name].Daily[new DayDate(_service.Timestamp)].Deviation.ShouldBe(30) // half hour
			);
		}

		[Fact]
		public void When_projecting_several_commits_on_several_days()
		{
			var firstDay = _service.Timestamp;
			var secondDay = firstDay.AddDays(1);

			_service
				.MasterCommit()
				.Advance(1.Hour()).MasterCommit()
				.Advance(1.Hour()).MasterCommit()
				.Advance(2.Hours()).MasterCommit()
				.Advance(1.Hour()).MasterCommit()
				.AdvanceTo(secondDay)
				.Advance(2.Hours()).MasterCommit()
				.Advance(2.Hours()).MasterCommit()
				.Advance(1.Hour()).MasterCommit();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].Daily[new DayDate(firstDay)].Median.ShouldBe(60), //1 hour
				() => _view[_service.Name].Daily[new DayDate(firstDay)].Deviation.ShouldBe(30), // half hour
				() => _view[_service.Name].Daily[new DayDate(secondDay)].Median.ShouldBe(120), //2 hours
				() => _view[_service.Name].Daily[new DayDate(secondDay)].Deviation.ShouldBe(676.165, tolerance: 0.005)
			);
		}
	}
}
