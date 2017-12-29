using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Projections;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildFailed : IProjectable
	{
		public BuildFailed(Notification notification)
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
