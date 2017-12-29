using System;
using Dockson.Domain.Transformers.Build;

namespace Dockson.Domain.Projections
{
	public class BuildIntervalProjection : IntervalProjection<BuildSucceeded>
	{
		public BuildIntervalProjection(Action<string, DayDate, TrendView> updateView)
			: base(updateView)
		{
		}
	}
}
