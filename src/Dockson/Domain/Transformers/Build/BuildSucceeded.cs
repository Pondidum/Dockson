using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Projections;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildSucceeded : IProjectable
	{
		public BuildSucceeded(BuildNotification notification)
		{
			notification.Tags.TryGetValue("commit", out var commit);
			
			Timestamp = notification.Timestamp;
			Groups = new HashSet<string>(
				notification.Groups.Append(notification.Name),
				StringComparer.OrdinalIgnoreCase);

			Name = notification.Name;
			CommitHash = commit;
			Version = notification.Version;
		}

		public DateTime Timestamp { get; }
		public HashSet<string> Groups { get; }
		public string Name { get; }
		public string CommitHash { get; }
		public string Version { get; }
	}
}
