using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Projections.BuildFailureRate;
using Dockson.Domain.Transformers.Build;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.BuildFailureRate
{
	public class BuildFailureRateProjectionTests
	{
		private readonly DateTime _yesterday;
		private readonly DateTime _today;

		private const string Group = "wat-service";

		private readonly BuildFailureRateView _view;
		private readonly BuildFailureRateProjection _projection;

		public BuildFailureRateProjectionTests()
		{
			_view = new BuildFailureRateView();
			_projection = new BuildFailureRateProjection(_view);

			var now = DateTime.Now;
			_today = now;
			_yesterday = now.AddDays(-1);
		}

		[Fact]
		public void When_a_build_is_successful()
		{
			_projection.Project(Success());

			_view[Group]
				.Daily[new DayDate(_today)]
				.FailureRate
				.ShouldBe(0);
		}

		[Fact]
		public void When_a_build_is_unsuccessful()
		{
			_projection.Project(Failure());

			_view[Group]
				.Daily[new DayDate(_today)]
				.FailureRate
				.ShouldBe(100);
		}


		[Fact]
		public void When_multiple_builds_are_a_mix()
		{
			_projection.Project(Failure());
			_projection.Project(Success());
			_projection.Project(Success());
			_projection.Project(Failure());

			_view[Group]
				.Daily[new DayDate(_today)]
				.FailureRate
				.ShouldBe(50);
		}

		[Fact]
		public void When_multiple_builds_are_a_mix_over_several_days()
		{
			_projection.Project(Failure(_yesterday));
			_projection.Project(Success(_yesterday));
			_projection.Project(Success(_yesterday));
			_projection.Project(Failure(_today));
			_projection.Project(Success(_today));
			_projection.Project(Failure(_today));

			var group = _view[Group];

			group.ShouldSatisfyAllConditions(
				() => group.Daily[new DayDate(_yesterday)].FailureRate.ShouldBe(33.33, tolerance: 0.01),
				() => group.Daily[new DayDate(_today)].FailureRate.ShouldBe(66.66, tolerance: 0.01)
			);
		}

		private BuildSucceeded Success(DateTime? when = null) => new BuildSucceeded(new Notification
		{
			Timestamp = when ?? _today,
			Name = Group
		});

		private BuildFailed Failure(DateTime? when = null) => new BuildFailed(new Notification
		{
			Timestamp = when ?? _today,
			Name = Group
		});
	}
}
