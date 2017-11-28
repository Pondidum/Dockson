using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Projections
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

	public class MasterCommitsProjection
	{
		private readonly Dictionary<string, Notification> _branchCommits;

		public MasterCommitsProjection()
		{
			_branchCommits = new Dictionary<string, Notification>();
		}

		private static bool IsMaster(string branch) => string.Equals(branch, "Master", StringComparison.OrdinalIgnoreCase);

		public void Project(Notification notification, Action<object> dispatch)
		{
			notification.Tags.TryGetValue("branch", out var branch);
			notification.Tags.TryGetValue("commit", out var commit);

			if (branch == null || commit == null)
				return;

			if (IsMaster(branch))
			{
				_branchCommits.Remove(commit, out var matchingCommit);

				if (matchingCommit != null)
					dispatch(new CommitToMaster(notification, matchingCommit));
			}
			else
			{
				_branchCommits.Add(commit, notification);
			}
		}
	}

	public class CommitToMaster
	{
		public CommitToMaster(Notification notification, Notification matchingCommit)
		{
		}
	}
}
