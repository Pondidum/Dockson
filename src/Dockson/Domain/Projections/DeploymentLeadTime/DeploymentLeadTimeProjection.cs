using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.Deployment;

namespace Dockson.Domain.Projections.DeploymentLeadTime
{
	public class DeploymentLeadTimeProjection : LeadTimeProjection<BuildSucceeded, ProductionDeployment>
	{
		public DeploymentLeadTimeProjection(LeadTimeView view)
			: base(view, build => build.Name + ":" + build.Version, deploy => deploy.Name + ":" + deploy.Version)
		{
		}
	}
}
