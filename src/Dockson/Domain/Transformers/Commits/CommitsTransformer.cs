using System;
using System.Collections.Generic;

namespace Dockson.Domain.Transformers.Commits
{
	public class CommitsTransformer : ITransformer<Notification>
	{
		private readonly Dictionary<string, Notification> _branchCommits;

		public CommitsTransformer()
		{
			_branchCommits = new Dictionary<string, Notification>();
		}

		private static bool IsMaster(string branch) => string.Equals(branch, "Master", StringComparison.OrdinalIgnoreCase);

		public void Transform(Notification notification, Action<object> dispatch)
		{
			if (notification.Type != Stages.Commit)
				return;

			notification.Tags.TryGetValue("branch", out var branch);
			notification.Tags.TryGetValue("commit", out var commit);

			if (branch == null || commit == null)
				return;

			if (IsMaster(branch))
			{
				_branchCommits.Remove(commit, out var matchingCommit);

				if (matchingCommit != null)
					dispatch(new MasterCommit(notification));
			}
			else
			{
				_branchCommits.Add(commit, notification);
				dispatch(new BranchCommit(notification));
			}
		}
	}
}
