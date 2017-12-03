using System;
using System.Collections.Generic;

namespace Dockson.Domain.Views
{
	public class MasterLeadTimeView
	{
		public HashSet<DateTime> Days { get; set; }
		public Dictionary<DateTime, double> Medians { get; set; }
		public Dictionary<DateTime, double> StandardDeviations { get; set; }

		public MasterLeadTimeView()
		{
			Days = new HashSet<DateTime>();
			Medians = new Dictionary<DateTime, double>();
			StandardDeviations = new Dictionary<DateTime, double>();
		}
	}
}
