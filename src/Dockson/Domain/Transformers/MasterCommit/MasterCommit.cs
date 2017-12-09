using System;
using System.Collections.Generic;

namespace Dockson.Domain.Transformers.MasterCommit
{
	public class MasterCommit
	{
		public MasterCommit(Notification masterCommit, Notification branchCommit)
		{
			Timestamp = masterCommit.Timestamp;
			LeadTime = masterCommit.Timestamp - branchCommit.Timestamp;
			Groups = masterCommit.Groups;	// don't bother copying it, this event is thrown away anyway
		}

		public DateTime Timestamp { get; }
		public TimeSpan LeadTime { get; }
		public HashSet<string> Groups { get; }
	}
}
