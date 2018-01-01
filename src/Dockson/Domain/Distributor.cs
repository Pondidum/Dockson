using System;
using System.Collections.Generic;
using Dockson.Domain.Projections;
using Dockson.Domain.Transformers;

namespace Dockson.Domain
{
	public class Distributor : IProjector
	{
		private readonly IStateStore _stateStore;
		private readonly ViewStore _viewStore;

		private readonly Cache<Type, List<Action<object, Action<object>>>> _transformers;
		private readonly Cache<Type, List<Action<object>>> _projections;

		public Distributor(IStateStore stateStore, ViewStore viewStore)
		{
			_stateStore = stateStore;
			_viewStore = viewStore;
			_transformers = new Cache<Type, List<Action<object, Action<object>>>>(key => new List<Action<object, Action<object>>>());
			_projections = new Cache<Type, List<Action<object>>>(key => new List<Action<object>>());
		}

		public void Project(Notification notification)
		{
			var events = new List<object>();
			var transformers = _transformers[notification.GetType()];

			foreach (var transformer in transformers)
				transformer(notification, events.Add);

			foreach (var @event in events)
			foreach (var project in _projections[@event.GetType()])
				project(@event);

			_stateStore.Save();
			_viewStore.Save();
		}

		public void AddTransformer<TState, TNotification>(ITransformer<TState, TNotification> transformer) where TState : new()
		{
			_transformers[typeof(TNotification)].Add((notification, dispatch) => transformer.Transform((TNotification)notification, dispatch));

			transformer.State = _stateStore.StateFor<TState>(transformer.GetType());
		}

		public void AddProjection<TState, TMessage>(IProjection<TState, TMessage> projection) where TState : new()
		{
			_projections[typeof(TMessage)].Add(message => projection.Project((TMessage)message));

			projection.State = _stateStore.StateFor<TState>(projection.GetType());
		}

		public void AddProjection<TState, TStart, TFinish>(IProjection<TState, TStart, TFinish> projection) where TState : new()
		{
			_projections[typeof(TStart)].Add(message => projection.Start((TStart)message));
			_projections[typeof(TFinish)].Add(message => projection.Finish((TFinish)message));

			projection.State = _stateStore.StateFor<TState>(projection.GetType());
		}
	}
}
