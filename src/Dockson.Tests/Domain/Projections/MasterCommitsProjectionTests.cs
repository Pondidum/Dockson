using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Projections
{
	public class MasterCommitsProjectionTests
	{
		private readonly MasterCommitsProjection _projection;
		private readonly DateTime _now;

		public MasterCommitsProjectionTests()
		{
			_projection = new MasterCommitsProjection();
			_now = DateTime.UtcNow;
		}

		[Fact]
		public void When_a_single_branch_commit()
		{
			var commits = new List<object>();

			_projection.Project(Commit(0, "feature-1", "commit-sha"), commits.Add);
			commits.ShouldBeEmpty();
		}

		[Fact]
		public void When_a_commit_makes_it_from_branch_to_master()
		{
			var commits = new List<object>();

			_projection.Project(Commit(0, "feature-1", "commit-sha"), commits.Add);
			_projection.Project(Commit(10, "master", "commit-sha"), commits.Add);

			commits.ShouldHaveSingleItem();
		}

		[Fact]
		public void When_the_notification_is_not_a_commit()
		{
			var commits = new List<object>();

			var featureCommit = Commit(0, "feature-1", "commit-sha");
			var masterCommit = Commit(10, "master", "commit-sha");

			featureCommit.Type = Stages.Build;
			masterCommit.Type = Stages.Build;

			_projection.Project(featureCommit, commits.Add);
			_projection.Project(masterCommit, commits.Add);

			commits.ShouldBeEmpty();
		}

		private Notification Commit(int offset, string branch, string hash) => new Notification
		{
			Type = Stages.Commit,
			TimeStamp = _now.AddMinutes(offset),
			Source = "github",
			Name = "SomeService",
			Version = "1.0.0",
			Tags = new Dictionary<string, string>
			{
				{ "commit", hash },
				{ "branch", branch }
			}
		};
	}
}
