using Dockson.Domain.Transformers.Build;

namespace Dockson.Domain.Projections.BuildInterval
{
	public class BuildIntervalProjection : IntervalProjection<BuildSucceeded>
	{
		public BuildIntervalProjection(IntervalView view)
			: base(view)
		{
		}
	}
}
