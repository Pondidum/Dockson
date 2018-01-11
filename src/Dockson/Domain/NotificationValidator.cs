using System.Collections.Generic;
using Dockson.Infrastructure.Validation;

namespace Dockson.Domain
{
	public class NotificationValidator : INotificationValidator<Notification>
	{
		public IEnumerable<string> Validate(Notification notification)
		{
			if (string.IsNullOrWhiteSpace(notification.Name))
				yield return "You must specify the project's name";
		}
	}
}
