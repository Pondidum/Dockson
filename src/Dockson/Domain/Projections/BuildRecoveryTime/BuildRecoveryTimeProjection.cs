using System;
using Dockson.Domain.Transformers.Build;

namespace Dockson.Domain.Projections.BuildRecoveryTime
{
	public class BuildRecoveryTimeProjection : LeadTimeProjection<BuildFailed, BuildSucceeded>
	{
		public BuildRecoveryTimeProjection(Action<string, DayDate, LeadTimeSummary> updateView)
			: base(updateView, failed => failed.Name, succeeded => succeeded.Name)
		{
		}
	}
}
