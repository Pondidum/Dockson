using System.Collections.Generic;
using Dockson.Infrastructure.Validation;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildNotificationValidator : INotificationValidator<BuildNotification>
	{
		public IEnumerable<string> Validate(BuildNotification notification)
		{
			if (string.IsNullOrWhiteSpace(notification.Status))
				yield return "You must specify the status of the build (success or failure)";
		}
	}
}
