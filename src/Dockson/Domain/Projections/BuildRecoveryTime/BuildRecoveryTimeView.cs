using System.Collections.Generic;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections.BuildRecoveryTime
{
	public class BuildRecoveryTimeView : Dictionary<string, GroupSummary<BuildRecoveryTimeSummary>>
	{
	}
}
