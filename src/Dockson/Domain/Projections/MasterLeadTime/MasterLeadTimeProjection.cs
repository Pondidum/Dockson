using Dockson.Domain.Transformers.Commits;

namespace Dockson.Domain.Projections.MasterLeadTime
{
	public class MasterLeadTimeProjection : LeadTimeProjection<BranchCommit, MasterCommit>
	{
		public MasterLeadTimeProjection(LeadTimeView view)
			: base(view, branch => branch.CommitHash, master => master.CommitHash)
		{
		}
	}
}
