using Dockson.Domain;
using Dockson.Domain.Projections;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class MasterLeadTimeProjectionTests
	{
		private readonly View _view;
		private readonly EventSource _service;

		public MasterLeadTimeProjectionTests()
		{
			var updater = new ViewStore();
			var projection = new MasterLeadTimeProjection(updater.UpdateMasterCommitLeadTime);

			_view = updater.View;
			_service = EventSource.For(projection);
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_service
				.BranchCommit()
				.Advance(1.Hour())
				.MasterCommit();
			var day = _service.CurrentDay;

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].MasterCommitLeadTime[day].Median.ShouldBe(1.Hour().TotalMinutes),
				() => _view[_service.Name].MasterCommitLeadTime[day].Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_commits()
		{
			var day = _service.CurrentDay;

			_service
				.BranchCommit()
				.Advance(1.Hour())
				.MasterCommit()
				.BranchCommit()
				.Advance(5.Hours())
				.MasterCommit();

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].MasterCommitLeadTime[day].Median.ShouldBe(3.Hours().TotalMinutes),
				() => _view[_service.Name].MasterCommitLeadTime[day].Deviation.ShouldBe(169.70, tolerance: 0.01)
			);
		}
	}
}
