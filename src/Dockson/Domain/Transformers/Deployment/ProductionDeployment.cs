using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Projections;

namespace Dockson.Domain.Transformers.Deployment
{
	public class ProductionDeployment : IProjectable
	{
		public ProductionDeployment(DeploymentNotification notification)
		{
			Timestamp = notification.Timestamp;
			Groups = new HashSet<string>(
				notification.Groups.Append(notification.Name),
				StringComparer.OrdinalIgnoreCase);

			Name = notification.Name;
			Version = notification.Version;
		}

		public DateTime Timestamp { get; }
		public HashSet<string> Groups { get; }
		public string Name { get; }
		public string Version { get; }
	}
}
