using System;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildTransformer : ITransformer
	{
		public void Transform(Notification notification, Action<object> dispatch)
		{
			if (notification.Type != Stages.Build)
				return;

			var isSuccess = notification.Status.EqualsIgnore("success");

			if (isSuccess)
				dispatch(new BuildSucceeded(notification));
			else
				dispatch(new BuildFailed(notification));
		}
	}
}
