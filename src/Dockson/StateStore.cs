﻿using System;

namespace Dockson
{
	public class StateStore : IStateStore
	{
		public TState StateFor<TState>(Type owner) where TState : new()
		{
			throw new NotImplementedException();
		}

		public void Save()
		{
		}

		public void Load()
		{
		}
	}
}