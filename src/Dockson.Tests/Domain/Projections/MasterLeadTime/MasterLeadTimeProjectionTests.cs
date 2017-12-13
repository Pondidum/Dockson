using System;
using Dockson.Domain;
using Dockson.Domain.Projections.MasterLeadTime;
using Dockson.Domain.Transformers.MasterCommit;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.MasterLeadTime
{
	public class MasterLeadTimeProjectionTests
	{
		private readonly MasterLeadTimeView _view;
		private readonly EventSource _service;

		public MasterLeadTimeProjectionTests()
		{
			_view = new MasterLeadTimeView();
			var projection = new MasterLeadTimeProjection(_view);
			_service = new EventSource(projection);
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_service.MasterCommit(sinceFeatureCommit: 1.Hour());
			var day = new DayDate(_service.Timestamp);

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].Daily[day].Median.ShouldBe(1.Hour().TotalMinutes),
				() => _view[_service.Name].Daily[day].Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_commits()
		{
			var day = new DayDate(_service.Timestamp);

			_service
				.MasterCommit(sinceFeatureCommit: 1.Hour())
				.Advance(4.Hours())
				.MasterCommit(sinceFeatureCommit: 5.Hours());

			_view.ShouldSatisfyAllConditions(
				() => _view[_service.Name].Daily[day].Median.ShouldBe(3.Hours().TotalMinutes),
				() => _view[_service.Name].Daily[day].Deviation.ShouldBe(169.70, tolerance: 0.01)
			);
		}
	}
}
