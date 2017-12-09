using System;
using Dockson.Domain;
using Dockson.Domain.Projections.BuildRecoveryTime;
using Dockson.Domain.Transformers.Build;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.BuildRecoveryTime
{
	public class BuildRecoveryTimeProjectionTests
	{
		private readonly DateTime _today;

		private const string Group = "wat-service";

		private readonly BuildRecoveryTimeView _view;
		private readonly BuildRecoveryTimeProjection _projection;

		public BuildRecoveryTimeProjectionTests()
		{
			_view = new BuildRecoveryTimeView();
			_projection = new BuildRecoveryTimeProjection(_view);

			var now = DateTime.Now;
			_today = now;
		}

		[Fact]
		public void When_a_build_recovers()
		{
			_projection.Project(BuildFixed(TimeSpan.FromMinutes(20)));

			var daySummary = _view[Group].Daily[new DayDate(_today)];

			daySummary.ShouldSatisfyAllConditions(
				() => daySummary.Median.ShouldBe(20),
				() => daySummary.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_multiple_builds_recovers()
		{
			_projection.Project(BuildFixed(TimeSpan.FromMinutes(20)));
			_projection.Project(BuildFixed(TimeSpan.FromMinutes(10)));

			var daySummary = _view[Group].Daily[new DayDate(_today)];

			daySummary.ShouldSatisfyAllConditions(
				() => daySummary.Median.ShouldBe(15),
				() => daySummary.Deviation.ShouldBe(7.071, tolerance: 0.001)
			);
		}

		[Fact]
		public void When_multiple_service_builds_recovers()
		{
			_projection.Project(BuildFixed(TimeSpan.FromMinutes(20), "service-1"));
			_projection.Project(BuildFixed(TimeSpan.FromMinutes(10), "service-2"));
			_projection.Project(BuildFixed(TimeSpan.FromMinutes(20), "service-2"));

			var serviceOne = _view["service-1"].Daily[new DayDate(_today)];

			serviceOne.ShouldSatisfyAllConditions(
				() => serviceOne.Median.ShouldBe(20),
				() => serviceOne.Deviation.ShouldBe(0)
			);

			var serviceTwo = _view["service-2"].Daily[new DayDate(_today)];

			serviceTwo.ShouldSatisfyAllConditions(
				() => serviceTwo.Median.ShouldBe(15),
				() => serviceTwo.Deviation.ShouldBe(7.071, tolerance: 0.001)
			);
		}

		private BuildFixed BuildFixed(TimeSpan fixTime, string group = Group) => new BuildFixed(
			new BuildFailed(new Notification
			{
				Name = group,
				Timestamp = _today,
				Groups = { group }
			}),
			new BuildSucceeded(new Notification
			{
				Name = group,
				Timestamp = _today.Add(fixTime),
				Groups = { group }
			})
		);
	}
}
