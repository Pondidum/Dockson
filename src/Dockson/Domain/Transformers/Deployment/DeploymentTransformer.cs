using System;

namespace Dockson.Domain.Transformers.Deployment
{
	public class DeploymentTransformer : ITransformer<DeploymentNotification>
	{
		public void Transform(DeploymentNotification notification, Action<object> dispatch)
		{
			if (notification.Type != Stages.Deploy)
				return;

			if (notification.Status.EqualsIgnore("success") == false)
				return;

			if (notification.Environment.EqualsIgnore("production"))
				dispatch(new ProductionDeployment(notification));
		}
	}
}
