using System;
using System.Collections.Generic;

namespace Dockson.Domain.Transformers.Build
{
	public class BuildTransformer : ITransformer
	{
		private readonly Dictionary<string, BuildFailed> _failures;

		public BuildTransformer()
		{
			_failures = new Dictionary<string, BuildFailed>();
		}

		public void Transform(Notification notification, Action<object> dispatch)
		{
			if (notification.Type != Stages.Build)
				return;

			var key = KeyFor(notification);
			var isSuccess = notification.Status.EqualsIgnore("success");

			if (isSuccess == false)
			{
				var fail = new BuildFailed(notification);
				_failures.TryAdd(key, fail);

				dispatch(fail);
			}

			if (isSuccess)
			{
				var success = new BuildSucceeded(notification);

				dispatch(success);

				if (_failures.Remove(key, out var fail))
					dispatch(new BuildFixed(fail, success));
			}
		}

		private static string KeyFor(Notification notification) => $"{notification.Name}:::${notification.Version}";
	}
}
