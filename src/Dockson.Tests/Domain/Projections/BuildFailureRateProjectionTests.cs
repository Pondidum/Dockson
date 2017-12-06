using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Events;
using Dockson.Domain.Projections;
using Dockson.Domain.Views;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class BuildFailureRateProjectionTests
	{
		private readonly DateTime _yesterday;
		private readonly DateTime _today;

		private const string Group = "wat-service";

		private readonly BuildFailureRateView _view;
		private readonly BuildFailureRateProjection _projection;
		private readonly List<object> _events;

		public BuildFailureRateProjectionTests()
		{
			_view = new BuildFailureRateView();
			_projection = new BuildFailureRateProjection(_view);
			_events = new List<object>();

			var now = DateTime.Now;
			_today = now;
			_yesterday = now.AddDays(-1);
		}

		[Fact]
		public void When_a_build_is_successful()
		{
			_projection.Project(Success(), _events.Add);

			_view[Group]
				.Daily[new DayDate(_today)]
				.FailureRate
				.ShouldBe(0);
		}

		[Fact]
		public void When_a_build_is_unsuccessful()
		{
			_projection.Project(Failure(), _events.Add);

			_view[Group]
				.Daily[new DayDate(_today)]
				.FailureRate
				.ShouldBe(100);
		}


		[Fact]
		public void When_multiple_builds_are_a_mix()
		{
			_projection.Project(Failure(), _events.Add);
			_projection.Project(Success(), _events.Add);
			_projection.Project(Success(), _events.Add);
			_projection.Project(Failure(), _events.Add);

			_view[Group]
				.Daily[new DayDate(_today)]
				.FailureRate
				.ShouldBe(50);
		}

		[Fact]
		public void When_multiple_builds_are_a_mix_over_several_days()
		{
			_projection.Project(Failure(_yesterday), _events.Add);
			_projection.Project(Success(_yesterday), _events.Add);
			_projection.Project(Success(_yesterday), _events.Add);
			_projection.Project(Failure(_today), _events.Add);
			_projection.Project(Success(_today), _events.Add);
			_projection.Project(Failure(_today), _events.Add);

			var group = _view[Group];

			group.ShouldSatisfyAllConditions(
				() => group.Daily[new DayDate(_yesterday)].FailureRate.ShouldBe(33.33, tolerance: 0.01),
				() => group.Daily[new DayDate(_today)].FailureRate.ShouldBe(66.66, tolerance: 0.01)
			);
		}

		private BuildSucceeded Success(DateTime? when = null) => new BuildSucceeded(new Notification
		{
			TimeStamp = when ?? _today,
			Groups = { Group }
		});

		private BuildFailed Failure(DateTime? when = null) => new BuildFailed(new Notification
		{
			TimeStamp = when ?? _today,
			Groups = { Group }
		});
	}
}
