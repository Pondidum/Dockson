using System;
using System.Collections.Generic;

namespace Dockson.Domain.Views
{
	public class MasterIntervalView
	{
		public Dictionary<DayDate, Summary> Daily { get; set; }
		public Dictionary<WeekDate, Summary> Weekly { get; set; }

		public MasterIntervalView()
		{
			Daily = new Dictionary<DayDate, Summary>();
			Weekly = new Dictionary<WeekDate, Summary>();
		}
	}
}
