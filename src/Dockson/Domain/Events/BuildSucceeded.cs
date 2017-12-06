using System;
using System.Collections.Generic;

namespace Dockson.Domain.Events
{
	public class BuildSucceeded
	{
		public BuildSucceeded(Notification notification)
		{
			TimeStamp = notification.TimeStamp;
			Groups = notification.Groups;
		}

		public DateTime TimeStamp { get; }
		public HashSet<string> Groups { get; }
	}
}