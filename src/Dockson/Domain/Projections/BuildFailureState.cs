using System;
using Dockson.Infrastructure;

namespace Dockson.Domain.Projections
{
	public class BuildFailureState
	{
		public BuildFailureState()
		{
			Builds = new Cache<string, Cache<DayDate, Counts>>(
				StringComparer.OrdinalIgnoreCase,
				key => new Cache<DayDate, Counts>(x => new Counts())
			);
		}

		public Cache<string, Cache<DayDate, Counts>> Builds { get; set; }
		
		public class Counts
		{
			public int Failures { get; set; }
			public int Successes { get; set; }
			public double Total => Successes + Failures;
		}
	}
}
