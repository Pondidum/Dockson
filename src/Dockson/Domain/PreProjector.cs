using System;

namespace Dockson.Domain
{
	public class PreProjector : IProjector
	{
		private readonly IProjector _internal;
		private readonly Func<DateTime> _getDate;

		public PreProjector(IProjector @internal, Func<DateTime> getDate = null)
		{
			_internal = @internal;
			_getDate = getDate ?? (() => DateTime.UtcNow);
		}

		public void Project(Notification notification)
		{
			if (notification.Timestamp == DateTime.MinValue)
				notification.Timestamp = _getDate();

			_internal.Project(notification);
		}
	}
}
