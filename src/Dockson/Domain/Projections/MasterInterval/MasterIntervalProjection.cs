using System;
using Dockson.Domain.Transformers.Commits;

namespace Dockson.Domain.Projections.MasterInterval
{
	public class MasterIntervalProjection : IntervalProjection<MasterCommit>
	{
		public MasterIntervalProjection(Action<string, DayDate, TrendView> updateView)
			: base(updateView)
		{
		}
	}
}
