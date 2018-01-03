using System;
using Dockson.Domain;

namespace Dockson.Infrastructure
{
	public class PreProjector : IProjector
	{
		private readonly IProjector _internal;

		public PreProjector(IProjector @internal)
		{
			_internal = @internal;
		}

		public void Project(Notification notification)
		{
			notification.Timestamp = DateTime.UtcNow;

			_internal.Project(notification);
		}
	}
}
