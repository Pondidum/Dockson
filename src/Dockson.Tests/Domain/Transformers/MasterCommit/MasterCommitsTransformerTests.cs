using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Transformers.MasterCommit;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Transformers.MasterCommit
{
	public class MasterCommitsTransformerTests
	{
		private readonly MasterCommitsTransformer _transformer;
		private readonly DateTime _now;

		public MasterCommitsTransformerTests()
		{
			_transformer = new MasterCommitsTransformer();
			_now = DateTime.UtcNow;
		}

		[Fact]
		public void When_a_single_branch_commit()
		{
			var commits = new List<object>();

			_transformer.Transform(Commit(0, "feature-1", "commit-sha"), commits.Add);
			commits.ShouldBeEmpty();
		}

		[Fact]
		public void When_a_commit_makes_it_from_branch_to_master()
		{
			var commits = new List<object>();

			_transformer.Transform(Commit(0, "feature-1", "commit-sha"), commits.Add);
			_transformer.Transform(Commit(10, "master", "commit-sha"), commits.Add);

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

			_transformer.Transform(featureCommit, commits.Add);
			_transformer.Transform(masterCommit, commits.Add);

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
