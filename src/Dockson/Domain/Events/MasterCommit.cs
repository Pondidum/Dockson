using System;
using System.Collections.Generic;

namespace Dockson.Domain.Events
{
	public class MasterCommit
	{
		public MasterCommit(Notification masterCommit, Notification branchCommit)
		{
			TimeStamp = masterCommit.TimeStamp;
			LeadTime = masterCommit.TimeStamp - branchCommit.TimeStamp;
			Groups = masterCommit.Groups;	// don't bother copying it, this event is thrown away anyway
		}

		public DateTime TimeStamp { get; }
		public TimeSpan LeadTime { get; }
		public HashSet<string> Groups { get; }
	}
}
