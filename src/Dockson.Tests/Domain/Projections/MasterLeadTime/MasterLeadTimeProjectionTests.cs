using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Projections.MasterLeadTime;
using Dockson.Domain.Views;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.MasterLeadTime
{
	public class MasterLeadTimeProjectionTests
	{
		private readonly View _view;
		private readonly EventSource _service;

		public MasterLeadTimeProjectionTests()
		{
			_view = new View();
			var projection = new MasterLeadTimeProjection(_view.UpdateMasterCommitLeadTime);
			_service = new EventSource(projection);
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_service
				.BranchCommit()
				.Advance(1.Hour())
				.MasterCommit();
			var day = new DayDate(_service.Timestamp);

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].MasterCommitLeadTime[day].Median.ShouldBe(1.Hour().TotalMinutes),
				() => _view[_service.Name].MasterCommitLeadTime[day].Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_commits()
		{
			var day = new DayDate(_service.Timestamp);

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
