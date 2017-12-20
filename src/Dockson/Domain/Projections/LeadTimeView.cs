using System.Collections.Generic;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections
{
	public class LeadTimeView : Dictionary<string, GroupSummary<LeadTimeSummary>>
	{
	}
}
