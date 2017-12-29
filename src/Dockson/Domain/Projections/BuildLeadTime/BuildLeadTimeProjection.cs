using System;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.Commits;

namespace Dockson.Domain.Projections.BuildLeadTime
{
	public class BuildLeadTimeProjection : LeadTimeProjection<MasterCommit, BuildSucceeded>
	{
		public BuildLeadTimeProjection(Action<string, DayDate, TrendView> updateView)
			: base(updateView, commit => commit.CommitHash, build => build.CommitHash)
		{
		}
	}
}
