using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.MasterCommit;

namespace Dockson.Domain.Projections.BuildLeadTime
{
	public class BuildLeadTimeProjection : LeadTimeProjection<MasterCommit, BuildSucceeded>
	{
		public BuildLeadTimeProjection(LeadTimeView view)
			: base(view, commit => commit.CommitHash, build => build.CommitHash)
		{
		}
	}
}
