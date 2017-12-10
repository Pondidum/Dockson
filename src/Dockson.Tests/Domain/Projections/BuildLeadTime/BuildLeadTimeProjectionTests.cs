using System;
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
		private const string Service = "some-service";
		private static readonly string CommitHash = Guid.NewGuid().ToString();

		private readonly BuildLeadTimeView _view;
		private readonly BuildLeadTimeProjection _projection;
		private readonly DateTime _now;

		public BuildLeadTimeProjectionTests()
		{
			_view = new BuildLeadTimeView();
			_projection = new BuildLeadTimeProjection(_view);
			_now = DateTime.UtcNow;
		}

		[Fact]
		public void When_projecting_one_commit()
		{
			_projection.Project(Commit());

			_view.ShouldBeEmpty();
		}

		[Fact]
		public void When_projecting_one_build()
		{
			_projection.Project(BuildSucceeded());

			_view.ShouldBeEmpty();
		}

		[Fact]
		public void When_projecting_a_commit_and_matching_build()
		{
			_projection.Project(Commit());
			_projection.Project(BuildSucceeded(when: _now.AddMinutes(20)));

			var summary = _view[Service].Daily[new DayDate(_now)];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(20),
				() => summary.Deviation.ShouldBe(0)
			);
		}

		[Fact]
		public void When_projecting_two_commits_and_matching_builds()
		{
			var firstCommit = Guid.NewGuid().ToString();
			var secondCommit = Guid.NewGuid().ToString();

			_projection.Project(Commit(firstCommit));
			_projection.Project(BuildSucceeded(firstCommit, _now.AddMinutes(20)));

			_projection.Project(Commit(secondCommit, when: _now.AddMinutes(40)));
			_projection.Project(BuildSucceeded(secondCommit, _now.AddMinutes(40 + 15)));

			var summary = _view[Service].Daily[new DayDate(_now)];

			summary.ShouldSatisfyAllConditions(
				() => summary.Median.ShouldBe(17.5),
				() => summary.Deviation.ShouldBe(3.53, tolerance: 0.01)
			);
		}

		[Fact]
		public void When_projecting_two_commits_and_matching_builds_for_different_services()
		{
			var commitOne = "sha-1";
			var commitTwo = "sha-2";
			var serviceOne = "service-1";
			var serviceTwo = "service-2";

			_projection.Project(Commit(hash: commitOne, service: serviceOne));
			_projection.Project(BuildSucceeded(hash: commitOne, service: serviceOne, when: _now.AddMinutes(20)));

			_projection.Project(Commit(hash: commitTwo, service: serviceTwo, when: _now.AddMinutes(40)));
			_projection.Project(BuildSucceeded(hash: commitTwo, service: serviceTwo, when: _now.AddMinutes(40 + 15)));

			var one = _view[serviceOne].Daily[new DayDate(_now)];
			var two = _view[serviceTwo].Daily[new DayDate(_now)];
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

		private MasterCommit Commit(string hash = null, string service = null, DateTime? when = null) => new MasterCommit(
			CreateNotification(hash, when, service, "master"),
			CreateNotification(hash, when, service, "feature-whatever")
		);

		private Notification CreateNotification(string hash, DateTime? timestamp, string service, string branch) => new Notification
		{
			Type = Stages.Commit,
			Timestamp = timestamp ?? _now,
			Source = "github",
			Name = service ?? Service,
			Version = "1.0.0",
			Groups = { Team },
			Tags =
			{
				{ "commit", hash ?? CommitHash },
				{ "branch", branch }
			}
		};

		private BuildSucceeded BuildSucceeded(DateTime? when = null) => BuildSucceeded(hash: null, service: null, when: when);

		private BuildSucceeded BuildSucceeded(string hash, DateTime when) => BuildSucceeded(hash: hash, service: null, when: when);

		private BuildSucceeded BuildSucceeded(string hash, string service, DateTime? when = null) => new BuildSucceeded(new Notification
		{
			Timestamp = when ?? _now,
			Name = service ?? Service,
			Groups = { Team },
			Tags =
			{
				{ "commit", hash ?? CommitHash }
			}
		});
	}
}
