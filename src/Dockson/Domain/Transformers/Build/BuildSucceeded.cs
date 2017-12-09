using System;
using System.Collections.Generic;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildSucceeded
	{
		public BuildSucceeded(Notification notification)
		{
			Timestamp = notification.Timestamp;
			Groups = notification.Groups;

			Name = notification.Name;
			Version = notification.Version;
		}

		public DateTime Timestamp { get; }
		public HashSet<string> Groups { get; }
		public string Name { get; }
		public string Version { get; }
	}
}
