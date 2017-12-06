using System;
using System.Collections.Generic;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildFailed
	{
		public BuildFailed(Notification notification)
		{
			TimeStamp = notification.TimeStamp;
			Groups = notification.Groups;
		}

		public DateTime TimeStamp { get; }
		public HashSet<string> Groups { get; }
	}
}