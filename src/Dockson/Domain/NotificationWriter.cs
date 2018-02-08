using Dockson.Infrastructure;
using Dockson.Storage;

namespace Dockson.Domain
{
	public class NotificationWriter : IProjector
	{
		private readonly NotificationStore _notificationStore;
		private readonly IProjector _inner;

		public NotificationWriter(NotificationStore notificationStore, IProjector inner)
		{
			_notificationStore = notificationStore;
			_inner = inner;
		}

		public void Project(Notification notification)
		{
			_notificationStore.Append(notification);
			_inner.Project(notification);
		}
	}

	public interface INotificationStore
	{
		void Append(Notification notification);
	}
}
