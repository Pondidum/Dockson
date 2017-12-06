using System.Collections.Generic;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections.BuildFailureRate
{
	public class BuildFailureRateView : Dictionary<string, GroupSummary<BuildFailureRateSummary>>
	{
	}
}
