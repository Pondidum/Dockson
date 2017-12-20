using System.Collections.Generic;
using Dockson.Domain.Views;

namespace Dockson.Domain.Projections
{
	public class IntervalView : Dictionary<string, GroupSummary<IntervalSummary>>
	{
	}
}
