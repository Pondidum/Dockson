using Dockson.Domain.Transformers.Commits;

namespace Dockson.Domain.Projections.MasterInterval
{
	public class MasterIntervalProjection : IntervalProjection<MasterCommit>
	{
		public MasterIntervalProjection(IntervalView view)
			: base(view)
		{
		}
	}
}
