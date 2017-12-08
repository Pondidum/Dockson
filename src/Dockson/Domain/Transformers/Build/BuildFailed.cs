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

			Name = notification.Name;
			Version = notification.Version;
		}

		public DateTime TimeStamp { get; }
		public HashSet<string> Groups { get; }
		public string Name { get; }
		public string Version { get; }
	}
}
