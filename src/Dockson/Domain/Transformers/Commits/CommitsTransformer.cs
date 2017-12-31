using System;
using System.Collections.Generic;

namespace Dockson.Domain.Transformers.Commits
{
	public class CommitsTransformer : ITransformer<Dictionary<string, CommitNotification>, CommitNotification>
	{
		public Dictionary<string, CommitNotification> State { get; set; }

		private static bool IsMaster(string branch) => string.Equals(branch, "Master", StringComparison.OrdinalIgnoreCase);

		public void Transform(CommitNotification notification, Action<object> dispatch)
		{
			if (notification.Type != Stages.Commit)
				return;

			var branch = notification.Branch;
			var commit = notification.Commit;

			if (branch == null || commit == null)
				return;

			if (IsMaster(branch))
			{
				State.Remove(commit, out var matchingCommit);

				if (matchingCommit != null)
					dispatch(new MasterCommit(notification));
			}
			else
			{
				State.Add(commit, notification);
				dispatch(new BranchCommit(notification));
			}
		}
	}
}
