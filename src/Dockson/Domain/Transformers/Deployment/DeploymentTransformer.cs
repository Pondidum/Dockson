using System;

namespace Dockson.Domain.Transformers.Deployment
{
	public class DeploymentTransformer : ITransformer<object, DeploymentNotification>
	{
		public object State { get; set; }

		public void Transform(DeploymentNotification notification, Action<object> dispatch)
		{
			if (notification.Status.EqualsIgnore("success") == false)
				return;

			if (notification.Environment.EqualsIgnore("production"))
				dispatch(new ProductionDeployment(notification));
		}
	}
}
