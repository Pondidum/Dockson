using System;
using System.Collections.Generic;
using Dockson.Infrastructure;

namespace Dockson.Domain.Projections
{
	public class LeadTimeState
	{
		public Dictionary<string, DateTime> Commits { get; set; }
		public Cache<string, List<double>> Times { get; set; }

		public LeadTimeState()
		{
			Commits = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
			Times = new Cache<string, List<double>>(
				StringComparer.OrdinalIgnoreCase,
				key => new List<double>());
		}
	}
}
