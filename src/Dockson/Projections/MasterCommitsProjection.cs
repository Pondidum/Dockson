using System;
using System.Collections.Generic;

namespace Dockson.Projections
{
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
					dispatch(new MasterCommit(notification, matchingCommit));
			}
			else
			{
				_branchCommits.Add(commit, notification);
			}
		}
	}

	public class MasterCommit
	{
		public MasterCommit(Notification masterCommit, Notification branchCommit)
		{
			TimeStamp = masterCommit.TimeStamp;
			LeadTime = masterCommit.TimeStamp - branchCommit.TimeStamp;
		}

		public DateTime TimeStamp { get; }
		public TimeSpan LeadTime { get; set; }
	}
}
