using System.Collections.Generic;

namespace Dockson.Domain.Views
{
	public class GroupSummary<TSummary>
	{
		public Dictionary<DayDate, TSummary> Daily { get; set; }

		public GroupSummary()
		{
			Daily = new Dictionary<DayDate, TSummary>();
		}
	}
}
