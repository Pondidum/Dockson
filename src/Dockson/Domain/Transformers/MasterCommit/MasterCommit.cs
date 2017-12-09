using System;
using System.Collections.Generic;
using System.Linq;

namespace Dockson.Domain.Transformers.MasterCommit
{
	public class MasterCommit
	{
		public MasterCommit(Notification masterCommit, Notification branchCommit)
		{
			Timestamp = masterCommit.Timestamp;
			LeadTime = masterCommit.Timestamp - branchCommit.Timestamp;
			Groups = new HashSet<string>(masterCommit.Groups.Append(masterCommit.Name));
		}

		public DateTime Timestamp { get; }
		public TimeSpan LeadTime { get; }
		public HashSet<string> Groups { get; }
	}
}
