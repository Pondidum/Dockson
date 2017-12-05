using System;
using System.Collections.Generic;

namespace Dockson.Domain.Views
{
	public class MasterIntervalView : Dictionary<string, GroupSummary>
	{
	}

	public class GroupSummary
	{
		public Dictionary<DayDate, Summary> Daily { get; set; }
		public Dictionary<WeekDate, Summary> Weekly { get; set; }

		public GroupSummary()
		{
			Daily = new Dictionary<DayDate, Summary>();
			Weekly = new Dictionary<WeekDate, Summary>();
		}
	}
}
