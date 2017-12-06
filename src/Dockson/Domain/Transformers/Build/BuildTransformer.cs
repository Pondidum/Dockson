using System;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildTransformer : ITransformer
	{
		public void Transform(Notification notification, Action<object> dispatch)
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
