using System.Collections.Generic;

namespace Dockson.Domain.Views
{
	public class BuildFailureRateGroupSummary
	{
		public Dictionary<DayDate, BuildFailureRateSummary> Daily { get; set; }

		public BuildFailureRateGroupSummary()
		{
			Daily = new Dictionary<DayDate, BuildFailureRateSummary>();
		}
	}
}
