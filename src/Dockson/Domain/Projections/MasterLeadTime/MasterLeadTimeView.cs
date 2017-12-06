using System.Collections.Generic;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections.MasterLeadTime
{
	public class MasterLeadTimeView : Dictionary<string, GroupSummary<MasterLeadTimeSummary>>
	{
	}
}
