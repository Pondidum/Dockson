using System;
using Dockson.Domain.Transformers.Commits;

namespace Dockson.Domain.Projections.MasterLeadTime
{
	public class MasterLeadTimeProjection : LeadTimeProjection<BranchCommit, MasterCommit>
	{
		public MasterLeadTimeProjection(Action<string, DayDate, TrendView> updateView)
			: base(updateView, branch => branch.CommitHash, master => master.CommitHash)
		{
		}
	}
}
