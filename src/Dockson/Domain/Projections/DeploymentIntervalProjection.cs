using System;
using Dockson.Domain.Transformers.Deployment;

namespace Dockson.Domain.Projections
{
	public class DeploymentIntervalProjection : IntervalProjection<ProductionDeployment>
	{
		public DeploymentIntervalProjection(Action<string, DayDate, TrendView> updateView)
			: base(updateView)
		{
		}
	}
}
