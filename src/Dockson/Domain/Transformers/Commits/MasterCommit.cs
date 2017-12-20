using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Projections;

namespace Dockson.Domain.Transformers.Commits
{
	public class MasterCommit : IProjectable
	{
		public MasterCommit(Notification masterCommit)
		{
			masterCommit.Tags.TryGetValue("commit", out var commit);

			Timestamp = masterCommit.Timestamp;
			Groups = new HashSet<string>(masterCommit.Groups.Append(masterCommit.Name));
			Name = masterCommit.Name;
			CommitHash = commit;
		}

		public DateTime Timestamp { get; }
		public string Name { get; }
		public HashSet<string> Groups { get; }
		public string CommitHash { get; }
	}
}
