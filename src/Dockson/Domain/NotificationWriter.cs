namespace Dockson.Domain
{
	public class NotificationWriter : IProjector
	{
		private readonly INotificationStore _notificationStore;
		private readonly IProjector _inner;

		public NotificationWriter(INotificationStore notificationStore, IProjector inner)
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
