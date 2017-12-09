using System;
using System.Collections.Generic;
using System.Linq;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildFixed
	{
		public BuildFixed(BuildFailed fail, BuildSucceeded success)
		{
			RecoveryTime = success.Timestamp - fail.Timestamp;
			Groups = new HashSet<string>(success.Groups.Append(success.Name));
			FixedTimestamp = success.Timestamp;
		}

		public TimeSpan RecoveryTime { get; }
		public IEnumerable<string> Groups { get; }
		public DateTime FixedTimestamp { get; }
	}
}
