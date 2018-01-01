using System;
using System.Collections.Generic;

namespace Dockson
{
	public class DictionaryStateStore : IStateStore
	{
		private readonly Dictionary<string, object> _stateStore;

		public DictionaryStateStore()
		{
			_stateStore = new Dictionary<string, object>();
		}

		public TState StateFor<TState>(Type owner) where TState : new()
		{
			_stateStore.TryAdd(owner.Name, new TState());
			return (TState)_stateStore[owner.Name];
		}

		public void Save()
		{
		}

		public void Load()
		{
		}
	}
}
