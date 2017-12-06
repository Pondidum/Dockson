using System;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildProjection
	{
		public void Project(Notification notification, Action<object> dispatch)
		{
			if (notification.Type != Stages.Build)
				return;

			if (notification.Status.EqualsIgnore("success"))
				dispatch(new BuildSucceeded(notification));
			else
				dispatch(new BuildFailed(notification));
		}
	}
}
