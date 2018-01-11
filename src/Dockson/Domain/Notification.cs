using System;
using System.Collections.Generic;

namespace Dockson.Domain
{
	public class Notification
	{
		public DateTime Timestamp { get; set; }
		public string Name { get; set; }

		public HashSet<string> Groups { get; set; }
		public Dictionary<string, string> Tags { get; set; }

		public Notification()
		{
			Groups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			Tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}
	}
}
