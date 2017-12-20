using Dockson.Domain.Transformers.Deployment;

namespace Dockson.Domain.Projections.DeploymentInterval
{
	public class DeploymentIntervalProjection : IntervalProjection<ProductionDeployment>
	{
		public DeploymentIntervalProjection(IntervalView view)
			: base(view)
		{
		}
	}
}
