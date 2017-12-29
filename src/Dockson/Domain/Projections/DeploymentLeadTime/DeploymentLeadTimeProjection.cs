using System;
using Dockson.Domain.Transformers.Build;
using Dockson.Domain.Transformers.Deployment;

namespace Dockson.Domain.Projections.DeploymentLeadTime
{
	public class DeploymentLeadTimeProjection : LeadTimeProjection<BuildSucceeded, ProductionDeployment>
	{
		public DeploymentLeadTimeProjection(Action<string, DayDate, TrendView> updateView)
			: base(updateView, build => build.Name + ":" + build.Version, deploy => deploy.Name + ":" + deploy.Version)
		{
		}
	}
}
