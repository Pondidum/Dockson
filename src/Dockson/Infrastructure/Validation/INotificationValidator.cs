using System.Collections.Generic;
using Dockson.Domain;

namespace Dockson.Infrastructure.Validation
{
	public interface INotificationValidator<TNotification> where TNotification : Notification
	{
		IEnumerable<string> Validate(TNotification notification);
	}
}