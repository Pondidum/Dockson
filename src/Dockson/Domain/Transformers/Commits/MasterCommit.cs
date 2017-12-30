using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Projections;

namespace Dockson.Domain.Transformers.Commits
{
	public class MasterCommit : IProjectable
	{
		public MasterCommit(CommitNotification masterCommit)
		{
			Timestamp = masterCommit.Timestamp;
			Groups = new HashSet<string>(masterCommit.Groups.Append(masterCommit.Name));
			Name = masterCommit.Name;
			CommitHash = masterCommit.Commit;
		}

		public DateTime Timestamp { get; }
		public string Name { get; }
		public HashSet<string> Groups { get; }
		public string CommitHash { get; }
	}
}
