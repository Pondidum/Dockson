using System;
using Dockson.Domain.Transformers.Build;

namespace Dockson.Domain.Projections.BuildInterval
{
	public class BuildIntervalProjection : IntervalProjection<BuildSucceeded>
	{
		public BuildIntervalProjection(Action<string, DayDate, IntervalSummary> updateView)
			: base(updateView)
		{
		}
	}
}
