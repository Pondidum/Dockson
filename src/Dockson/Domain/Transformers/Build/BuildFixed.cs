using System;
using System.Collections.Generic;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildFixed
	{
		public BuildFixed(BuildFailed fail, BuildSucceeded success)
		{
			RecoveryTime = success.Timestamp - fail.Timestamp;
			Groups = success.Groups;
			FixedTimestamp = success.Timestamp;
		}

		public TimeSpan RecoveryTime { get; }
		public IEnumerable<string> Groups { get; }
		public DateTime FixedTimestamp { get; }
	}
}
