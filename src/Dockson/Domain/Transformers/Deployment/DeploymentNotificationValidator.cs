using System.Collections.Generic;
using Dockson.Infrastructure.Validation;

namespace Dockson.Domain.Transformers.Deployment
{
	public class DeploymentNotificationValidator : INotificationValidator<DeploymentNotification>
	{
		public IEnumerable<string> Validate(DeploymentNotification notification)
		{
			if (string.IsNullOrWhiteSpace(notification.Version))
				yield return "You must specify the project's version";

			if (string.IsNullOrWhiteSpace(notification.Environment))
				yield return "You must specify the project's deployment environment (Production, QA, Test etc)";

			if (string.IsNullOrWhiteSpace(notification.Status))
				yield return "You must specify the project's deployment status (success, failure etc)";
		}
	}
}
