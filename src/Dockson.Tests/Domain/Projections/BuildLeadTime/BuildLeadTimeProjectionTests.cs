using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dockson.Domain;
using Dockson.Domain.Projections.BuildLeadTime;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.MasterCommit;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections.BuildLeadTime
{
	public class BuildLeadTimeProjectionTests
	{
		private const string Team = "team-one";

		private readonly BuildLeadTimeView _view;
		private readonly BuildLeadTimeProjection _projection;
		private readonly DateTime _now;

		private readonly EventSource _serviceOne;
		private readonly EventSource _serviceTwo;

		public BuildLeadTimeProjectionTests()
		{
			_view = new BuildLeadTimeView();
			_projection = new BuildLeadTimeProjection(_view);
			_now = DateTime.UtcNow;

			_serviceOne = new EventSource { Name = "service-one", Groups = { Team } };
			_serviceTwo = new EventSource { Name = "service-two", Groups = { Team } };
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_projection.Project(_serviceOne.MasterCommit());

			_view.ShouldBeEmpty();
		}

		[Fact]
		public void When_projecting_one_build()
		{
			_projection.Project(_serviceOne.BuildSucceeded());

			_view.ShouldBeEmpty();
		}

		[Fact]
		public void When_projecting_a_commit_and_matching_build()
		{
			_projection.Project(_serviceOne.MasterCommit());
			_serviceOne.Advance(TimeSpan.FromMinutes(20));
			_projection.Project(_serviceOne.BuildSucceeded());

			var summary = _view[_serviceOne.Name].Daily[new DayDate(_now)];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(20),
				() => summary.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_commits_and_matching_builds()
		{
			_projection.Project(_serviceOne.MasterCommit());
			_serviceOne.Advance(TimeSpan.FromMinutes(20));
			_projection.Project(_serviceOne.BuildSucceeded());

			_serviceOne.Advance(TimeSpan.FromMinutes(20));
			_serviceOne.CommitHash = Guid.NewGuid().ToString();
			
			_projection.Project(_serviceOne.MasterCommit());
			_serviceOne.Advance(TimeSpan.FromMinutes(15));
			_projection.Project(_serviceOne.BuildSucceeded());

			var summary = _view[_serviceOne.Name].Daily[new DayDate(_now)];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(17.5),
				() => summary.Deviation.ShouldBe(3.53, tolerance: 0.01)
			);
		}

		[Fact]
		public void When_projecting_two_commits_and_matching_builds_for_different_services()
		{
			_projection.Project(_serviceOne.MasterCommit());
			_serviceOne.Advance(TimeSpan.FromMinutes(20));
			_projection.Project(_serviceOne.BuildSucceeded());

			_serviceTwo.Advance(TimeSpan.FromMinutes(40));
			_projection.Project(_serviceTwo.MasterCommit());
			_serviceTwo.Advance(TimeSpan.FromMinutes(15));
			_projection.Project(_serviceTwo.BuildSucceeded());

			var one = _view[_serviceOne.Name].Daily[new DayDate(_now)];
			var two = _view[_serviceTwo.Name].Daily[new DayDate(_now)];
			var team = _view[Team].Daily[new DayDate(_now)];

			_view.ShouldSatisfyAllConditions(
				() => one.Median.ShouldBe(20),
				() => one.Deviation.ShouldBe(0),
				() => two.Median.ShouldBe(15),
				() => two.Deviation.ShouldBe(0),
				() => team.Median.ShouldBe(17.5),
				() => team.Deviation.ShouldBe(3.53, tolerance: 0.01)
			);
		}
	}

	internal class EventSource
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public string CommitHash { get; set; }
		public DateTime Timestamp { get; set; }
		public HashSet<string> Groups { get; set; }

		public EventSource()
		{
			Name = "some-service";
			Version = "2.13.4";
			CommitHash = Guid.NewGuid().ToString();
			Timestamp = DateTime.UtcNow;
			Groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}

		public void Advance(TimeSpan time) => Timestamp = Timestamp.Add(time);
		
		
		public MasterCommit MasterCommit(TimeSpan? sinceFeatureCommit = null) => new MasterCommit(
			CreateNotification(Timestamp, "master"),
			CreateNotification(Timestamp.Subtract(sinceFeatureCommit ?? TimeSpan.Zero), "feature-whatever")
		);

		private Notification CreateNotification(DateTime timestamp, string branch) => new Notification
		{
			Type = Stages.Commit,
			Timestamp = timestamp,
			Source = "github",
			Name =Name,
			Version = "1.0.0",
			Groups = Groups,
			Tags =
			{
				{ "commit", CommitHash },
				{ "branch", branch }
			}
		};
		public BuildSucceeded BuildSucceeded(Action<Notification> modify = null)
		{
			var notification = new Notification
			{
				Name = Name,
				Version = Version,
				Timestamp = Timestamp,
				Groups = Groups,
				Tags =
				{
					{ "commit", CommitHash }
				}
			};
			modify?.Invoke(notification);

			return new BuildSucceeded(notification);
		}

	}
}
