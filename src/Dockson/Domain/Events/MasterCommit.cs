using System;

namespace Dockson.Domain.Events
{
	public class MasterCommit
	{
		public MasterCommit(Notification masterCommit, Notification branchCommit)
		{
			TimeStamp = masterCommit.TimeStamp;
			LeadTime = masterCommit.TimeStamp - branchCommit.TimeStamp;
		}

		public DateTime TimeStamp { get; }
		public TimeSpan LeadTime { get; }
	}
}
