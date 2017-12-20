using System;

namespace Dockson.Domain.Transformers.Deployment
{
	public class DeploymentTransformer : ITransformer
	{
		public void Transform(Notification notification, Action<object> dispatch)
		{
			if (notification.Type != Stages.Deploy)
				return;

			if (notification.Status.EqualsIgnore("success") == false)
				return;

			if (notification.Tags.TryGetValue("environment", out var environment) && environment.EqualsIgnore("production"))
				dispatch(new ProductionDeployment(notification));
		}
	}
}
