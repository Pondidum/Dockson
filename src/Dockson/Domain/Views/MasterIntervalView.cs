using System;
using System.Collections.Generic;

namespace Dockson.Domain.Views
{
	public class MasterIntervalView
	{
		public Dictionary<DateTime, Summary> Daily { get; set; }
		public Dictionary<WeekDate, Summary> Weekly { get; set; }

		public MasterIntervalView()
		{
			Daily = new Dictionary<DateTime, Summary>();
			Weekly = new Dictionary<WeekDate, Summary>();
		}
	}
}
