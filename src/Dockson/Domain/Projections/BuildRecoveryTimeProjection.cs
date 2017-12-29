using System;
using Dockson.Domain.Transformers.Build;

namespace Dockson.Domain.Projections
{
	public class BuildRecoveryTimeProjection : LeadTimeProjection<BuildFailed, BuildSucceeded>
	{
		public BuildRecoveryTimeProjection(Action<string, DayDate, TrendView> updateView)
			: base(updateView, failed => failed.Name, succeeded => succeeded.Name)
		{
		}
	}
}
