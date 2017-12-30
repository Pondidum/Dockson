using Dockson.Domain;
using Dockson.Domain.Projections;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class MasterIntervalProjectionTests
	{
		private readonly View _view;
		private readonly EventSource _service;

		public MasterIntervalProjectionTests()
		{
			_view = new View();
			var projection = new MasterIntervalProjection(_view.UpdateMasterCommitInterval);

			_service = new EventSource(projection);
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_service
				.BranchCommit()
				.MasterCommit();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].MasterCommitInterval[_service.CurrentDay].Median.ShouldBe(0),
				() => _view[_service.Name].MasterCommitInterval[_service.CurrentDay].Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_commits_on_the_same_day()
		{
			_service
				.BranchCommit()
				.MasterCommit()
				.Advance(1.Hour())
				.BranchCommit()
				.MasterCommit();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].MasterCommitInterval[_service.CurrentDay].Median.ShouldBe(60), //1 hour
				() => _view[_service.Name].MasterCommitInterval[_service.CurrentDay].Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_several_commits_on_the_same_day()
		{
			_service
				.BranchCommit().MasterCommit()
				.Advance(1.Hour()).BranchCommit().MasterCommit()
				.Advance(1.Hour()).BranchCommit().MasterCommit()
				.Advance(2.Hours()).BranchCommit().MasterCommit()
				.Advance(1.Hour()).BranchCommit().MasterCommit();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].MasterCommitInterval[_service.CurrentDay].Median.ShouldBe(60), //1 hour
				() => _view[_service.Name].MasterCommitInterval[_service.CurrentDay].Deviation.ShouldBe(30) // half hour
			);
		}

		[Fact]
		public void When_projecting_several_commits_on_several_days()
		{
			var firstDay = _service.Timestamp;
			var secondDay = firstDay.AddDays(1);

			_service
				.BranchCommit().MasterCommit()
				.Advance(1.Hour()).BranchCommit().MasterCommit()
				.Advance(1.Hour()).BranchCommit().MasterCommit()
				.Advance(2.Hours()).BranchCommit().MasterCommit()
				.Advance(1.Hour()).BranchCommit().MasterCommit()
				.AdvanceTo(secondDay)
				.Advance(2.Hours()).BranchCommit().MasterCommit()
				.Advance(2.Hours()).BranchCommit().MasterCommit()
				.Advance(1.Hour()).BranchCommit().MasterCommit();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].MasterCommitInterval[new DayDate(firstDay)].Median.ShouldBe(60), //1 hour
				() => _view[_service.Name].MasterCommitInterval[new DayDate(firstDay)].Deviation.ShouldBe(30), // half hour
				() => _view[_service.Name].MasterCommitInterval[new DayDate(secondDay)].Median.ShouldBe(120), //2 hours
				() => _view[_service.Name].MasterCommitInterval[new DayDate(secondDay)].Deviation.ShouldBe(676.165, tolerance: 0.005)
			);
		}
	}
}
