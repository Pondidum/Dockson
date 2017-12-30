using System;

namespace Dockson.Domain.Transformers
{
	public interface ITransformer<TNotification>
	{
		void Transform(TNotification notification, Action<object> dispatch);
	}
}
