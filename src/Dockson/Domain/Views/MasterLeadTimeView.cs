using System;
using System.Collections.Generic;

namespace Dockson.Domain.Views
{
	public class MasterLeadTimeView
	{
		public HashSet<DayDate> Days { get; set; }
		public Dictionary<DayDate, double> Medians { get; set; }
		public Dictionary<DayDate, double> StandardDeviations { get; set; }

		public MasterLeadTimeView()
		{
			Days = new HashSet<DayDate>();
			Medians = new Dictionary<DayDate, double>();
			StandardDeviations = new Dictionary<DayDate, double>();
		}
	}
}
