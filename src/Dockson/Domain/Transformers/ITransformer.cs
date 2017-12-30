using System;

namespace Dockson.Domain.Transformers
{
	public interface ITransformer<TNotification> where TNotification : Notification
	{
		void Transform(TNotification notification, Action<object> dispatch);
	}
}
