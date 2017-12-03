using System;
using System.Collections.Generic;

namespace Dockson.Domain
{
	public class Notification
	{
		public DateTime TimeStamp { get; set; }
		public string Source { get; set; }
		public Stages Type { get; set; }

		public string Name { get; set; }
		public string Version { get; set; }
		public string Status { get; set; }
		
		public HashSet<string> Groups { get; set; }
		public Dictionary<string, string> Tags { get; set; }
	}
}
