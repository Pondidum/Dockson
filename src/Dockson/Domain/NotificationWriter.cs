using Dockson.Infrastructure;

namespace Dockson.Domain
{
	public class NotificationWriter : IProjector
	{
		private readonly SequencedWriter<Notification> _notificationStore;
		private readonly IProjector _inner;

		public NotificationWriter(SequencedWriter<Notification> notificationStore, IProjector inner)
		{
			_notificationStore = notificationStore;
			_inner = inner;
		}

		public void Project(Notification notification)
		{
			_notificationStore.Write(notification);
			_inner.Project(notification);
		}
	}

	public interface INotificationStore
	{
		void Append(Notification notification);
	}
}
