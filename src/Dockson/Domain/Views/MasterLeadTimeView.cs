using System;
using System.Collections.Generic;

namespace Dockson.Domain.Views
{
	public class MasterLeadTimeView
	{
		public Dictionary<DayDate, Summary> Daily { get; set; }

		public MasterLeadTimeView()
		{
			Daily = new Dictionary<DayDate, Summary>();
		}
	}
}
