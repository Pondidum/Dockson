using System;

namespace Dockson.Domain.Transformers
{
	public interface ITransformer<TState, TNotification> where TState : new()
	{
		TState State { get; set; }
		void Transform(TNotification notification, Action<object> dispatch);
	}
}
