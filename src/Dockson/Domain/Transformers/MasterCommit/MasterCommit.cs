using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Projections;

namespace Dockson.Domain.Transformers.MasterCommit
{
	public class MasterCommit : IProjectable
	{
		public MasterCommit(Notification masterCommit, Notification branchCommit)
		{
			masterCommit.Tags.TryGetValue("commit", out var commit);
			
			Timestamp = masterCommit.Timestamp;
			LeadTime = masterCommit.Timestamp - branchCommit.Timestamp;
			Groups = new HashSet<string>(masterCommit.Groups.Append(masterCommit.Name));
			Name = masterCommit.Name;
			CommitHash = commit;
		}

		public DateTime Timestamp { get; }
		public TimeSpan LeadTime { get; }
		public string Name { get; }
		public HashSet<string> Groups { get; }
		public string CommitHash { get; }
	}
}
