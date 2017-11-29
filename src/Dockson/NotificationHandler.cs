using System;
using System.Collections.Generic;
using System.Linq;

namespace Dockson
{
	public class NotificationHandler
	{
		private readonly List<Action<Notification>> _projections;

		public NotificationHandler(IEnumerable<Action<Notification>> projections)
		{
			_projections = projections.ToList();
		}

		public void Handle(Notification notification)
		{
			//_store.write(notification);

			_projections.ForEach(projection => projection(notification));
		}
	}
}
