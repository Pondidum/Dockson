using System.Linq;
using Dockson.Domain.Transformers.Commits;
using Shouldly;
using Xunit;

namespace Dockson.Tests.Domain.Transformers.Commits
{
	public class CommitNotificationValidatorTests
	{
		private string[] Validate(CommitNotification notification) => new CommitNotificationValidator()
			.Validate(notification)
			.ToArray();

		[Fact]
		public void When_the_commit_hash_is_missing()
		{
			Validate(new CommitNotification { Branch = "wat" }).ShouldBe(new[]
			{
				"You must specify the project's Commit Hash"
			});
		}

		[Fact]
		public void When_the_branch_is_missing()
		{
			Validate(new CommitNotification { Commit = "hash" }).ShouldBe(new[]
			{
				"You must specify the project's branch"
			});
		}

		[Fact]
		public void When_both_commit_and_branch_are_missing()
		{
			Validate(new CommitNotification()).ShouldBe(new[]
			{
				"You must specify the project's branch",
				"You must specify the project's Commit Hash"
			});
		}

		[Fact]
		public void When_both_commit_and_branch_are_present()
		{
			Validate(new CommitNotification { Branch = "wat", Commit = "hashhhh"}).ShouldBeEmpty();
		}
	}
}
