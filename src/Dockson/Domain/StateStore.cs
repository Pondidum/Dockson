﻿using System;

namespace Dockson.Domain
{
	public class StateStore : IStateStore
	{
		public TState StateFor<TState>(Type owner) where TState : new()
		{
			return new TState();
		}

		public void Save()
		{
		}

		public void Load()
		{
		}
	}
}