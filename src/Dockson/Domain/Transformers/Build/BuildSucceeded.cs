using System;
using System.Collections.Generic;
using System.Linq;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildSucceeded
	{
		public BuildSucceeded(Notification notification)
		{
			Timestamp = notification.Timestamp;
			Groups = new HashSet<string>(notification.Groups.Append(notification.Name));

			Name = notification.Name;
		}

		public DateTime Timestamp { get; }
		public HashSet<string> Groups { get; }
		public string Name { get; }
	}
}
