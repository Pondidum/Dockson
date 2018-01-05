using System;
using System.Collections.Generic;
using System.IO;
using Dockson.Domain;
using Dockson.Infrastructure;
using Newtonsoft.Json;

namespace Dockson.Storage
{
	public class StateStore : IStateStore
	{
		private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			 TypeNameHandling = TypeNameHandling.Auto
		};

		private readonly IFileSystem _fileSystem;
		private readonly string _stateFile;

		private Dictionary<string, object> _state;

		public StateStore(IFileSystem fileSystem, Settings settings)
		{
			_fileSystem = fileSystem;
			_stateFile = Path.Combine(settings.StoragePath, "state.json");
		}

		public void Load()
		{
			var json = _fileSystem.ReadFile(_stateFile);

			_state = string.IsNullOrWhiteSpace(json) == false
				? JsonConvert.DeserializeObject<Dictionary<string, object>>(json, Settings)
				: new Dictionary<string, object>();
		}

		public void Save()
		{
			var json = JsonConvert.SerializeObject(_state, Settings);
			_fileSystem.WriteFile(_stateFile, json);
		}

		public TState StateFor<TState>(Type owner) where TState : new()
		{
			_state.TryAdd(owner.Name, new TState());
			return (TState)_state[owner.Name];
		}
	}
}
