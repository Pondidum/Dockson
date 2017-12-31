using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Transformers;

namespace Dockson
{
	public class Distributor
	{
		private readonly Dictionary<string, object> _stateStore;
		private readonly Cache<Type, List<Action<object, Action<object>>>> _transformers;
		private readonly Cache<Type, List<Action<object>>> _projections;

		public Distributor(Dictionary<string, object> stateStore = null)
		{
			_stateStore = stateStore ?? new Dictionary<string, object>();

			_transformers = new Cache<Type, List<Action<object, Action<object>>>>(key => new List<Action<object, Action<object>>>());
			_projections = new Cache<Type, List<Action<object>>>(key => new List<Action<object>>());
		}

		public void Project(Notification notification)
		{
			var events = new List<object>();

			_transformers[notification.GetType()].ForEach(tx => tx(notification, events.Add));

			foreach (var @event in events)
				_projections[@event.GetType()].ForEach(project => project(@event));
		}

		public void AddTransformer<T>(ITransformer<T> transformer)
		{
			_transformers[typeof(T)].Add((notification, dispatch) => transformer.Transform((T)notification, dispatch));
		}

		public void AddProjection<TState, TMessage>(IProjection<TState, TMessage> projection) where TState : new()
		{
			_projections[typeof(TMessage)].Add(message => projection.Project((TMessage)message));

			projection.State = StateFor<TState>(projection.GetType());
		}

		public void AddProjection<TState, TStart, TFinish>(IProjection<TState, TStart, TFinish> projection) where TState : new()
		{
			_projections[typeof(TStart)].Add(message => projection.Start((TStart)message));
			_projections[typeof(TFinish)].Add(message => projection.Finish((TFinish)message));

			projection.State = StateFor<TState>(projection.GetType());
		}

		private TState StateFor<TState>(Type projection) where TState : new()
		{
			_stateStore.TryAdd(projection.Name, new TState());
			return (TState)_stateStore[projection.Name];
		}
	}
}
