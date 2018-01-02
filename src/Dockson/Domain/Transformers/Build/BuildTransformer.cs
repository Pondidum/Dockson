using System;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildTransformer : ITransformer<object, BuildNotification>
	{
		public object State { get; set; }

		public void Transform(BuildNotification notification, Action<object> dispatch)
		{
			var isSuccess = notification.Status.EqualsIgnore("success");

			if (isSuccess)
				dispatch(new BuildSucceeded(notification));
			else
				dispatch(new BuildFailed(notification));
		}
	}
}
