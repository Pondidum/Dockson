using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Projections;

namespace Dockson.Domain.Transformers.Commits
{
	public class BranchCommit : IProjectable
	{
		public BranchCommit(CommitNotification notification)
		{
			CommitHash = notification.Commit;
			Timestamp = notification.Timestamp;
			Groups = new HashSet<string>(
				notification.Groups.Append(notification.Name),
				StringComparer.OrdinalIgnoreCase);
		}

		public string CommitHash { get; }
		public DateTime Timestamp { get; }
		public HashSet<string> Groups { get; }
	}
}
