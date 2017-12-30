using System;
using System.Collections.Generic;
using Dockson.Domain;
using Dockson.Domain.Projections;
using Dockson.Domain.Transformers;

namespace Dockson
{
	public class Distributor
	{
		private readonly ITransformer<Notification>[] _transformers;
		private readonly Cache<Type, List<Action<object>>> _projections;

		public Distributor(ITransformer<Notification>[] transformers)
		{
			_transformers = transformers;
			_projections = new Cache<Type, List<Action<object>>>(key => new List<Action<object>>());
		}

		public void Project(Notification notification)
		{
			var events = new List<object>();

			foreach (var transformer in _transformers)
				transformer.Transform(notification, events.Add);

			foreach (var @event in events)
				_projections[@event.GetType()].ForEach(project => project(@event));
		}

		public void AddProjection<T>(IProjection<T> projection)
		{
			_projections[typeof(T)].Add(message => projection.Project((T)message));
		}

		public void AddProjection<TStart, TFinish>(IProjection<TStart, TFinish> projection)
		{
			_projections[typeof(TStart)].Add(message => projection.Start((TStart)message));
			_projections[typeof(TFinish)].Add(message => projection.Finish((TFinish)message));
		}
	}
}
