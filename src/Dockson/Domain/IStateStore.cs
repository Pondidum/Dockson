using System;

namespace Dockson.Domain
{
	public interface IStateStore
	{
		TState StateFor<TState>(Type owner) where TState : new();
		void Save();
		void Load();
	}
}
