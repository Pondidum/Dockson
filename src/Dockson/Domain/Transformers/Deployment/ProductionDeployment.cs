using System;
using System.Collections.Generic;
using System.Linq;
using Dockson.Domain.Projections;

namespace Dockson.Domain.Transformers.Deployment
{
	public class ProductionDeployment : IProjectable
	{
		public ProductionDeployment(Notification notification)
		{
			Timestamp = notification.Timestamp;
			Groups = new HashSet<string>(
				notification.Groups.Append(notification.Name),
				StringComparer.OrdinalIgnoreCase);
		}

		public DateTime Timestamp { get; }
		public HashSet<string> Groups { get; }
	}
}
