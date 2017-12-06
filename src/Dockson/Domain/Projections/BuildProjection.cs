using System;
using Dockson.Domain.Events;

namespace Dockson.Domain.Projections
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
