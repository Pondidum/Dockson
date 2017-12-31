﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain;
using Dockson.Domain.Transformers.Commits;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Transformers.Commits
{
	public class CommitsTransformerTests
	{
		private readonly CommitsTransformer _transformer;
		private readonly DateTime _now;

		public CommitsTransformerTests()
		{
			_transformer = new CommitsTransformer
			{
				State = new Dictionary<string, CommitNotification>()
			};

			_now = DateTime.UtcNow;
		}

		[Fact]
		public void When_a_single_branch_commit()
		{
			var commits = new List<object>();

			_transformer.Transform(Commit(0, "feature-1", "commit-sha"), commits.Add);

			commits
				.ShouldHaveSingleItem()
				.ShouldBeOfType<BranchCommit>();
		}

		[Fact]
		public void When_a_commit_makes_it_from_branch_to_master()
		{
			var commits = new List<object>();

			_transformer.Transform(Commit(0, "feature-1", "commit-sha"), commits.Add);
			_transformer.Transform(Commit(10, "master", "commit-sha"), commits.Add);

			commits.Select(c => c.GetType()).ShouldBe(new[]
			{
				typeof(BranchCommit),
				typeof(MasterCommit)
			});
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

		private CommitNotification Commit(int offset, string branch, string hash) => new CommitNotification
		{
			Type = Stages.Commit,
			Timestamp = _now.AddMinutes(offset),
			Source = "github",
			Name = "SomeService",
			Version = "1.0.0",
			Commit =  hash,
			Branch = branch
		};
	}
}
